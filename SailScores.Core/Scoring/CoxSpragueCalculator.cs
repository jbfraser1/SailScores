﻿using SailScores.Api.Enumerations;
using SailScores.Core.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;

namespace SailScores.Core.Scoring
{

    // Scoring system based on
    // https://www.ussailing.org/competition/rules-officiating/racing-rules/scoring-a-long-series/

    public class CoxSpragueCalculator : BaseScoringCalculator
    {
        public CoxSpragueCalculator(ScoringSystem scoringSystem) : base(scoringSystem)
        {
            CompetitorComparer = new CoxSpragueCompComparer();
        }

        protected override decimal? GetBasicScore(IEnumerable<Score> allScores, Score currentScore)
        {
            int starters = allScores.Count(s =>
                s.Race == currentScore.Race
                 && CountsAsStarted(s));


            int baseScore =
                allScores
                    .Count(s =>
                        currentScore.Place.HasValue
                        && s.Race == currentScore.Race
                        && s.Place <= currentScore.Place
                        && !ShouldAdjustOtherScores(s)
                        );

            // removed code to handle race result ties: this should treat both as the same place.

            return CoxSpragueTable.GetScore( baseScore, starters);
        }

        // for Cox-Sprague this is most coded results: they depend on number of starters
        protected override void CalculateRaceDependentScores(SeriesResults resultsWorkInProgress, SeriesCompetitorResults compResults)
        {
            
            foreach (var race in resultsWorkInProgress.SailedRaces)
            {
                var score = compResults.CalculatedScores[race];
                var scoreCode = GetScoreCode(score.RawScore);
                if (scoreCode != null && CameToStart(score.RawScore)
                    && scoreCode.Formula != TIE_FORMULANAME)
                {
                    var starters = race.Scores.Count(s => CountsAsStarted(s));
                    score.ScoreValue = CoxSpragueTable.GetScore(starters + 1, starters);
                    //if (IsTrivialCalculation(scoreCode))
                    //{
                    //    score.ScoreValue = GetTrivialScoreValue(score);
                    //}
                    //else if (IsRaceBasedValue(scoreCode))
                    //{
                    //    score.ScoreValue = CalculateRaceBasedValue(score, race);
                    //}
                }
            }
        }

        /// The series score for each boat:
        /// divide the sum of her race scores by the sum of the points
        /// she would have scored if she had placed first in every race in
        /// which she competed
        protected override void CalculateTotals(
            SeriesResults results,
            IEnumerable<Score> scores)
        {
            results.IsPercentSystem = true;
            results.PercentRequired = ScoringSystem.ParticipationPercent;
            var raceCount = results.Races.Where(r => (r.State ?? RaceState.Raced) == RaceState.Raced
                || r.State == RaceState.Preliminary).Count();
            var requiredRaces = raceCount * ((ScoringSystem.ParticipationPercent ?? 0) / 100m);

            Dictionary<Guid, int> starterCounts = new Dictionary<Guid, int>();
            foreach (Race r in results.SailedRaces)
            {
                starterCounts[r.Id] = r.Scores.Count(s => CountsAsStarted(s));
            }
            foreach (var comp in results.Competitors)
            {
                var currentCompResults = results.Results[comp];
                if (currentCompResults.CalculatedScores.Where(s => s.Value.RawScore.Code != DEFAULT_CODE).Count()
                    < requiredRaces)
                {
                    currentCompResults.TotalScore = null;
                }
                else
                {
                    // racesToExclude should include discards and DNCs
                    var racesToExclude = currentCompResults
                        .CalculatedScores
                        .Where(s => s.Value.Discard ||
                        ( !String.IsNullOrEmpty(s.Value.RawScore.Code) && !(GetScoreCode(s.Value.RawScore.Code).CameToStart ?? false)))
                        .Select(s => s.Key.Id);

                    int perfectScore = 0;
                    foreach (Race r in results.SailedRaces)
                    {
                        if (racesToExclude.Contains(r.Id))
                        {
                            continue;
                        }

                        perfectScore += CoxSpragueTable.GetScore(1, starterCounts[r.Id]);
                    }

                    var compTotal = currentCompResults
                        .CalculatedScores.Values
                        .Sum(s => !s.Discard ? (s.ScoreValue ?? 0.0m) : 0.0m);

                    currentCompResults.PointsEarned = compTotal;
                    currentCompResults.PointsPossible = perfectScore;
                    if (perfectScore == 0)
                    {
                        currentCompResults.TotalScore = 0;
                    }
                    else
                    {
                        currentCompResults.TotalScore = compTotal * 100 / perfectScore;
                    }
                }
            }
        }

        protected override void DiscardScores(
            SeriesResults resultsWorkInProgress,
            SeriesCompetitorResults compResults)
        {
            int numOfDiscards = GetNumberOfDiscards(resultsWorkInProgress);

            var compResultsOrdered = compResults.CalculatedScores.Values.OrderBy(s => s.ScoreValue)
                .ThenBy(s => s.RawScore.Race.Date)
                .ThenBy(s => s.RawScore.Race.Order)
                .Where(s => GetScoreCode(s.RawScore)?.Discardable ?? true);
            foreach (var score in compResultsOrdered.Take(numOfDiscards))
            {
                score.Discard = true;
            }
        }

        protected override void CalculateOverrides(SeriesResults resultsWorkInProgress, SeriesCompetitorResults compResults)
        {
            foreach (var race in resultsWorkInProgress.SailedRaces)
            {
                var score = compResults.CalculatedScores[race];
                var defaultScore = GetDefaultScore(race, resultsWorkInProgress);
                if (score?.ScoreValue != null && score.ScoreValue < defaultScore)
                {
                    score.ScoreValue = defaultScore;
                }
            }
        }

        protected override decimal? GetPenaltyScore(CalculatedScore score, Race race, ScoreCode scoreCode)
        {
            var dnfScore = GetDnfScore(race) ?? 0;
            var fleetSize = race.Scores.Where(s => CameToStart(s)).Count();
            var percentAdjustment = Convert.ToDecimal(scoreCode?.FormulaValue ?? 20);
            var percent = Math.Round(fleetSize * percentAdjustment / 100m, MidpointRounding.AwayFromZero);

            return Math.Max(dnfScore, (score.ScoreValue ?? 0) - percent);
        }

    }
}
