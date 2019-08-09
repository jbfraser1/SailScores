﻿using AutoMapper;
using SailScores.Core.Model;
using SailScores.Core.Services;
using SailScores.Web.Models.SailScores;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core = SailScores.Core;

namespace SailScores.Web.Services
{
    public class RegattaService : IRegattaService
    {
        private readonly Core.Services.IClubService _clubService;
        private readonly Core.Services.IRegattaService _coreRegattaService;
        private readonly IMapper _mapper;

        public RegattaService(
            Core.Services.IClubService clubService,
            Core.Services.IRegattaService coreRegattaService,
            IMapper mapper)
        {
            _clubService = clubService;
            _coreRegattaService = coreRegattaService;
            _mapper = mapper;
        }

        public async Task<IEnumerable<RegattaSummary>> GetAllRegattaSummaryAsync(string clubInitials)
        {
            var coreObject = await _clubService.GetFullClub(clubInitials);
            var orderedRegattas = coreObject.Regattas
                .OrderByDescending(s => s.Season.Start)
                .ThenBy(s => s.StartDate)
                .ThenBy(s => s.Name);
            return _mapper.Map<IList<RegattaSummary>>(orderedRegattas);
        }

        public async Task<Regatta> GetRegattaAsync(string clubInitials, string season, string regattaName)
        {
            // todo
            return await _coreRegattaService.GetRegattaAsync(clubInitials, season, regattaName);
        }
    }
}
