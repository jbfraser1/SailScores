﻿using SailScores.Api.Enumerations;
using SailScores.Core.Model;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SailScores.Core.Scoring
{
    public class AppendixACalculator : IScoringCalculator
    {
        private const string AVERAGE_FORMULANAME = "AVE";
        private const string SERIESCOMPETITORS_FORMULANAME = "SER+";

        private const StringComparison CASE_INSENSITIVE = StringComparison.InvariantCultureIgnoreCase;


        private readonly ScoringSystem _scoringSystem;

        //Code to use if no result is found or if scorecode is not found in the system.
        //  This will be used if code defined in a child scoring system is used but the
        // series is scored with the ancestor
        private readonly string DEFAULT_CODE = "DNC";

        public AppendixACalculator(ScoringSystem scoringsystem)
        {
            _scoringSystem = scoringsystem;
        }

        public SeriesResults CalculateResults(Series series)
        {
            var returnResults = new SeriesResults
            {
                Races = series.Races.OrderBy(r => r.Date).ThenBy(r => r.Order).ToList(),
                Competitors = series
                    .Races
                    .SelectMany(
                        r => r
                            .Scores
                            .Select(s => s.Competitor))
                    .Distinct()
                    .ToList(),
                Results = new Dictionary<Competitor, SeriesCompetitorResults>()
            };

            SetScores(returnResults,
                series
                .Races
                .SelectMany(
                    r => r
                        .Scores));
            var sailedRaceCount = returnResults.GetSailedRaceCount();
            returnResults.NumberOfDiscards = GetNumberOfDiscards(sailedRaceCount);
            return returnResults;
        }

        public void SetScores(SeriesResults resultsWorkInProgress, IEnumerable<Score> scores)
        {
            ValidateScores(resultsWorkInProgress, scores);
            ClearRawScores(scores);
            foreach (var comp in resultsWorkInProgress.Competitors)
            {
                SeriesCompetitorResults compResults = GenerateBasicScores(comp, scores);
                CalculateCodedResults(resultsWorkInProgress, compResults);
                DiscardScores(resultsWorkInProgress, compResults);
                CalculateTotal(compResults);

                resultsWorkInProgress.Results[comp] = compResults;
            }
            CalculateRanks(resultsWorkInProgress);
            resultsWorkInProgress.Competitors = ReorderCompetitors(resultsWorkInProgress);
        }


        private IList<Competitor> ReorderCompetitors(SeriesResults results)
        {
            return results.Competitors.OrderBy(c => results.Results[c].Rank).ToList();
        }

        private void CalculateRanks(SeriesResults resultsWorkInProgress)
        {
            var orderedComps = resultsWorkInProgress.Results.Values
                .OrderBy(s => s, new SeriesCompetitorResultComparer());

            int i = 1;
            foreach (var comp in orderedComps)
            {
                comp.Rank = i;

                i++;
            }
        }

        private void CalculateCodedResults(SeriesResults resultsWorkInProgress, SeriesCompetitorResults compResults)
        {
            //Fill in DNCs
            foreach (var race in resultsWorkInProgress.SailedRaces)
            {
                if (!compResults.CalculatedScores.ContainsKey(race))
                {
                    compResults.CalculatedScores.Add(race,
                        new CalculatedScore
                        {
                            RawScore = new Score
                            {
                                Code ="DNC",
                                Competitor = compResults.Competitor,
                                Race = race
                            }
                        });
                }
            }

            CalculateRaceDependentScores(resultsWorkInProgress, compResults);
            CalculateSeriesDependentScores(resultsWorkInProgress, compResults);
        }


        private void CalculateRaceDependentScores(SeriesResults resultsWorkInProgress, SeriesCompetitorResults compResults)
        {
            //calculate non-average codes first
            foreach (var race in resultsWorkInProgress.SailedRaces)
            {
                var score = compResults.CalculatedScores[race];
                var scoreCode = GetScoreCode(score.RawScore);
                if (scoreCode != null)
                {
                    if (IsTrivialCalculation(scoreCode))
                    {
                        score.ScoreValue = GetTrivialScoreValue(score);
                    } else if (IsRaceBasedValue(scoreCode))
                    {
                        score.ScoreValue = CalculateRaceBasedValue(score, race);
                    }
                }
            }
        }

        private void CalculateSeriesDependentScores(SeriesResults resultsWorkInProgress, SeriesCompetitorResults compResults)
        {
            foreach (var race in resultsWorkInProgress.SailedRaces)
            {
                var score = compResults.CalculatedScores[race];
                var scoreCode = GetScoreCode(score.RawScore);
                if (score != null && IsSeriesBasedScore(scoreCode))
                {
                    score.ScoreValue = CalculateSeriesBasedValue(score, compResults);

                }
            }
        }

        private bool IsSeriesBasedScore(ScoreCode scoreCode)
        {
            // defaults to false if not a coded score.
            bool average = scoreCode?.Formula?.Equals(AVERAGE_FORMULANAME, CASE_INSENSITIVE) 
                ?? false;
            bool seriesCompPlus = scoreCode?.Formula?.Equals(SERIESCOMPETITORS_FORMULANAME, CASE_INSENSITIVE)
                ?? false;
            return average || seriesCompPlus;
        }

        private decimal? CalculateSeriesBasedValue(CalculatedScore score, SeriesCompetitorResults compResults)
        {
            // right now the only kind of series based value is Average, so not much to do here.
            return CalculateAverage(compResults);
        }

        private bool IsTrivialCalculation(ScoreCode scoreCode)
        {
            return scoreCode.Formula.Equals("MAN", CASE_INSENSITIVE);
        }

        private decimal? GetTrivialScoreValue(CalculatedScore score)
        {
            if (score.RawScore.Place.HasValue)
            {
                return Convert.ToDecimal(score.RawScore.Place);
            }
            return null;
        }

        private bool IsRaceBasedValue(ScoreCode scoreCode)
        {
            return scoreCode.Formula.Equals("FIN+", CASE_INSENSITIVE)
                || scoreCode.Formula.Equals("PLC%", CASE_INSENSITIVE);
        }

        private decimal? CalculateRaceBasedValue(CalculatedScore score, Race race)
        {
            var scoreCode = GetScoreCode(score.RawScore);
            switch (scoreCode.Formula.ToUpperInvariant())
            {
                case "FIN+":
                    return race.Scores.Where(s => CountsAsStarted(s)).Count() + 
                        scoreCode.FormulaValue;
                case "CTS+":
                    return race.Scores.Where(s => CameToStart(s)).Count() +
                        scoreCode.FormulaValue;
                case "PLC%":
                    return GetPenaltyScore(score, race, scoreCode);
            }
            throw new InvalidOperationException("Score code definition issue with race based score code.");
        }

        private decimal? GetPenaltyScore(CalculatedScore score, Race race, ScoreCode scoreCode)
        {
            var dnfScore = GetDnfScore(race) ?? 1;
            var percentAdjustment = Convert.ToDecimal(scoreCode?.FormulaValue ?? 20);
            var percent = Math.Round(dnfScore * percentAdjustment / 100m, MidpointRounding.AwayFromZero);
            return Math.Min(dnfScore, percent + (score.RawScore.Place ?? 0));
        }

        private decimal? GetDnfScore(Race race)
        {
            var dnfCode = _scoringSystem.ScoreCodes.FirstOrDefault(c => c.Name.Equals("DNF", StringComparison.InvariantCultureIgnoreCase));
            return race.Scores.Where(s => CountsAsStarted(s)).Count() +
                        dnfCode.FormulaValue;
        }

        private decimal? CalculateAverage(
            SeriesCompetitorResults compResults)
        {
            //int numRealResults = compResults.CalculatedScores
            //        .Values.Count(s =>
            //            String.IsNullOrWhiteSpace(s.RawScore.Code));
            int numAverages = compResults.CalculatedScores
                    .Values.Count(s =>
                        IsAverage(s.RawScore.Code));
            int discards = GetNumberOfDiscards(compResults.CalculatedScores.Count);

            var average = compResults.CalculatedScores.Values
                .Where(s => (s.ScoreValue ?? 0m) != 0m && !IsAverage(s.RawScore.Code))
                .OrderBy(s => s.ScoreValue)
                .Take(compResults.CalculatedScores.Count - numAverages - discards)
                .Average(s => s.ScoreValue) ?? 0m;

            return Math.Round(average, 1, MidpointRounding.AwayFromZero);

        }

        private bool IsAverage(string code)
        {
            if (String.IsNullOrWhiteSpace(code))
            {
                return false;
            }
            return GetScoreCode(code).Formula.Equals(AVERAGE_FORMULANAME, CASE_INSENSITIVE);
        }

        private bool CountsAsStarted(Score s)
        {
            if (String.IsNullOrWhiteSpace(s.Code) &&
                (s.Place ?? 0) != 0)
            {
                return true;
            }
            var scoreCode = GetScoreCode(s);
            return scoreCode.Started ?? false;
        }

        private bool CameToStart(Score s)
        {
            if (String.IsNullOrWhiteSpace(s.Code) &&
                (s.Place ?? 0) != 0)
            {
                return true;
            }
            var scoreCode = GetScoreCode(s);
            return scoreCode.CameToStart ?? false;
        }

        private void CalculateTotal(SeriesCompetitorResults compResults)
        {
            compResults.TotalScore = compResults.CalculatedScores.Values.Sum(s => !s.Discard ? ( s.ScoreValue ?? 0.0m) : 0.0m);
        }

        private void DiscardScores(SeriesResults resultsWorkInProgress, SeriesCompetitorResults compResults)
        {
            int numOfDiscards = GetNumberOfDiscards(resultsWorkInProgress);

            var compResultsOrdered = compResults.CalculatedScores.Values.OrderByDescending(s => s.ScoreValue)
                .ThenBy(s => s.RawScore.Race.Date)
                .ThenBy(s => s.RawScore.Race.Order)
                .Where(s => GetScoreCode(s.RawScore)?.Discardable ?? true);
            foreach (var score in compResultsOrdered.Take(numOfDiscards))
            {
                score.Discard = true;
            }
        }

        private int GetNumberOfDiscards(SeriesResults resultsWorkInProgress)
        {
            return GetNumberOfDiscards(resultsWorkInProgress.GetSailedRaceCount());

        }
        private int GetNumberOfDiscards(int numberOfRaces)
        {
            if(numberOfRaces == 0)
            {
                return 0;
            }
            var discardStrings = _scoringSystem.DiscardPattern.Split(',');
            string selectedString;
            if(numberOfRaces > discardStrings.Length)
            {
                 selectedString = discardStrings[discardStrings.Length - 1];
            } else
            {
                selectedString = discardStrings[numberOfRaces - 1];
            }

            return int.Parse(selectedString);
        }


        public void ValidateScores(SeriesResults results, IEnumerable<Score> scores)
        {
            bool allRacesFound = scores.All(s => results.Races.Any(
                r => r.Id == s.RaceId
                    || r == s.Race));
            bool allCompetitorsFound = scores.All(s => results.Competitors.Any(
                c => c.Id == s.CompetitorId
                    || c == s.Competitor ));

            //Used to check and make sure all score codes were found. but no more.

            if (!allRacesFound)
            {
                throw new InvalidOperationException(
                    "A score for a race that is not in the series was provided to SeriesCalculator");
            }

            if (!allCompetitorsFound)
            {
                throw new InvalidOperationException(
                    "A score for a competitor that is not in the series was provided to SeriesCalculator");

            }
        }

        private SeriesCompetitorResults GenerateBasicScores(Competitor comp, IEnumerable<Score> scores)
        {
            var returnResults = new SeriesCompetitorResults
            {
                Competitor = comp,
                CalculatedScores = new Dictionary<Race, CalculatedScore>()
            };
            foreach (var score in scores.Where(s => s.Competitor == comp))
            {
                if((score.Race?.State ?? RaceState.Raced) != RaceState.Raced) {
                    continue;
                }
                returnResults.CalculatedScores[score.Race] = new CalculatedScore
                {
                    Discard = false,
                    RawScore = score,
                    ScoreValue = score.Place
                };
                returnResults.CalculatedScores[score.Race].ScoreValue =
                    scores
                        .Count(s =>
                            score.Place.HasValue
                            && s.Race == score.Race
                            && s.Place < score.Place
                            && !ShouldAdjustOtherScores(s)
                            ) + 1;

                // if this is one, no tie. (if zero Place doesn't have a value (= coded.))
                int numTied = scores.Count(s =>
                    score.Place.HasValue
                    && s.Race == score.Race
                    && s.Place == score.Place
                    && !ShouldAdjustOtherScores(s));
                if(numTied > 1) {
                    int total = 0;
                    for (int i = 0; i< numTied; i++)
                    {
                        total += ((int)score.Place + i);
                    }
                    returnResults.CalculatedScores[score.Race].ScoreValue = (decimal)total / numTied;
                }
            }

            return returnResults;
        }

        private bool ShouldAdjustOtherScores(Score score)
        {
            return !String.IsNullOrWhiteSpace(score.Code)
            && (GetScoreCode(score)?.AdjustOtherScores ?? true);
        }

        private void ClearRawScores(IEnumerable<Score> scores)
        {
            foreach(var score in scores)
            {
                if (!ShouldPreserveScore(score))
                {
                    score.Place = null;
                }
            }
        }

        private bool ShouldPreserveScore(Score score)
        {
            return String.IsNullOrWhiteSpace(score.Code)
                || ( GetScoreCode(score)?.PreserveResult ?? true);
        }

        private ScoreCode GetScoreCode(Score score)
        {
            return GetScoreCode(score.Code);
        }

        private ScoreCode GetScoreCode(string scoreCodeName)
        {
            if (String.IsNullOrWhiteSpace(scoreCodeName))
            {
                return null;
            }
            var returnScoreCode = _scoringSystem.ScoreCodes
                    .SingleOrDefault(c =>
                        c.Name.Equals(scoreCodeName, StringComparison.InvariantCultureIgnoreCase));

            if(returnScoreCode == null)
            {
                returnScoreCode = GetScoreCode(DEFAULT_CODE);
            }
            return returnScoreCode;
        }
    }

}



