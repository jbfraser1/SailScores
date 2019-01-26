﻿using AutoMapper;
using Microsoft.EntityFrameworkCore;
using SailScores.Database;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using AutoMapper.QueryableExtensions;
using System.Linq;
using SailScores.Core.Model;
using Db = SailScores.Database.Entities;

namespace SailScores.Core.Services
{
    public class SeasonService : ISeasonService
    {
        private readonly ISailScoresContext _dbContext;
        private readonly IMapper _mapper;

        public SeasonService(
            ISailScoresContext dbContext,
            IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }

        public async Task Delete(Guid seasonId)
        {
            var dbSeason = await _dbContext.Seasons.SingleAsync(c => c.Id == seasonId);
            _dbContext.Seasons.Remove(dbSeason);
            await _dbContext.SaveChangesAsync();
        }

        public async Task SaveNew(Season season)
        {
            var dbSeason =_mapper.Map<Db.Season>(season);
            dbSeason.Id = Guid.NewGuid();
            _dbContext.Seasons.Add(dbSeason);
            await _dbContext.SaveChangesAsync();

        }

        public async Task Update(Season season)
        {
            var existingSeason = await _dbContext.Seasons.SingleAsync(c => c.Id == season.Id);

            existingSeason.Name = season.Name;
            existingSeason.Start = season.Start;
            existingSeason.End = season.End;
            await _dbContext.SaveChangesAsync();
        }
    }
}
