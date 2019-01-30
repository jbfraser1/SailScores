﻿using SailScores.Core.Model;
using SailScores.Web.Models.SailScores;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SailScores.Web.Services
{
    public interface ISeriesService
    {
        Task<IEnumerable<SeriesSummary>> GetAllSeriesSummaryAsync(string clubInitials);
        Task<Core.Model.Series> GetSeriesAsync(string clubInitials, string season, string seriesName);
        Task SaveNew(SeriesWithOptionsViewModel model);
        Task Update(SeriesWithOptionsViewModel model);
        Task DeleteAsync(Guid id);
    }
}