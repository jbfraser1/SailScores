﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SailScores.Core.Services;
using SailScores.Web.Models.SailScores;
using SailScores.Web.Services;

namespace SailScores.Web.Controllers
{
    public class RaceController : Controller
    {

        private readonly IClubService _clubService;
        private readonly Web.Services.IRaceService _raceService;
        private readonly Services.IAuthorizationService _authService;
        private readonly IMapper _mapper;

        public RaceController(
            IClubService clubService,
            Web.Services.IRaceService raceService,
            Services.IAuthorizationService authService,
            IMapper mapper)
        {
            _clubService = clubService;
            _raceService = raceService;
            _authService = authService;
            _mapper = mapper;
        }

        public async Task<ActionResult> Index(string clubInitials)
        {
            var races = await _raceService.GetAllRaceSummariesAsync(clubInitials);

            return View(new ClubCollectionViewModel<RaceSummaryViewModel>
            {
                List = races,
                ClubInitials = clubInitials
            });
        }

        public async Task<ActionResult> Details(string clubInitials, Guid id)
        {
            var race = await _raceService.GetSingleRaceDetailsAsync(clubInitials, id);
            return View(new ClubItemViewModel<RaceViewModel>
            {
                Item = race,
                ClubInitials = clubInitials
            });
        }

        public async Task<ActionResult> Create(string clubInitials)
        {
            RaceWithOptionsViewModel race = await _raceService.GetBlankRaceWithOptions(clubInitials);

            return View(race);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(string clubInitials, RaceWithOptionsViewModel race)
        {
            try
            {
                var club = (await _clubService.GetClubs(true)).Single(c => c.Initials == clubInitials);
                if (!await _authService.CanUserEdit(User, club.Id))
                {
                    return Unauthorized();
                }
                race.ClubId = club.Id;
                await _raceService.SaveAsync(race);

                return RedirectToAction(nameof(Edit), "Admin");
            }
            catch
            {
                return View();
            }
        }

        public async Task<ActionResult> Edit(string clubInitials, Guid id)
        {
            var club = await _clubService.GetFullClub(clubInitials);
            if (!await _authService.CanUserEdit(User, club.Id))
            {
                return Unauthorized();
            }
            var race = await _raceService.GetSingleRaceDetailsAsync(clubInitials, id);
            if (race.ClubId != club.Id)
            {
                return Unauthorized();
            }
            var raceWithOptions = _mapper.Map<RaceWithOptionsViewModel>(race);

            return View(raceWithOptions);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(Guid id, RaceWithOptionsViewModel race)
        {
            try
            {
                if (!await _authService.CanUserEdit(User, race.ClubId))
                {
                    return Unauthorized();
                }
                await _raceService.SaveAsync(race);

                return RedirectToAction(nameof(Edit), "Admin");
            }
            catch
            {
                return View();
            }
        }

        [HttpGet]
        // GET: Competitor/Delete/5
        public async Task<ActionResult> Delete(string clubInitials, Guid id)
        {
            var club = (await _clubService.GetClubs(true)).Single(c => c.Initials == clubInitials);
            if (!await _authService.CanUserEdit(User, club.Id))
            {
                return Unauthorized();
            }
            var race = await _raceService.GetSingleRaceDetailsAsync(clubInitials, id);
            if (race.ClubId != club.Id)
            {
                return Unauthorized();
            }
            return View(race);
        }

        // POST: Competitor/Delete/5
        [HttpPost]
        [ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> PostDelete(string clubInitials, Guid id)
        {
            try
            {
                var club = await _clubService.GetFullClub(clubInitials);
                if (!await _authService.CanUserEdit(User, club.Id)
                    || !club.Competitors.Any(c => c.Id == id))
                {
                    return Unauthorized();
                }
                await _raceService.Delete(id);

                return RedirectToAction(nameof(Edit), "Admin");
            }
            catch
            {
                return View();
            }
        }

    }
}