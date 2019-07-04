﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SailScores.Core.Model;

namespace SailScores.Core.Services
{
    public interface IScoringService
    {
        Task<IEnumerable<ScoreCode>> GetScoreCodesAsync(Guid clubId);
        Task<IList<ScoringSystem>> GetScoringSystemsAsync(Guid clubId, bool includeBaseSystems);
        Task<ScoringSystem> GetScoringSystemAsync(Guid scoringSystemId);
        Task<ScoringSystem> GetScoringSystemAsync(Series series);
    }
}