﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using SailScores.Core.Model;
using SailScores.Core.Services;
using SailScores.Web.Models.SailScores;

namespace SailScores.Web.Controllers
{
    [Authorize]
    public class CompetitorController : Controller
    {
        private readonly IClubService _clubService;
        private readonly ICompetitorService _competitorService;
        private readonly IMapper _mapper;
        private readonly Services.IAuthorizationService _authService;
        private readonly Services.IAdminTipService _adminTipService;

        public CompetitorController(
            IClubService clubService,
            ICompetitorService competitorService,
            Services.IAuthorizationService authService,
            Services.IAdminTipService adminTipService,
            IMapper mapper)
        {
            _clubService = clubService;
            _competitorService = competitorService;
            _authService = authService;
            _adminTipService = adminTipService;
            _mapper = mapper;
        }

        // GET: Competitor
        public ActionResult Index(string clubInitials)
        {
            return View();
        }

        // GET: Competitor/Details/5
        public ActionResult Details(string clubInitials, Guid id)
        {
            return View();
        }

        // GET: Competitor/Create
        public async Task<ActionResult> Create(
            string clubInitials,
            string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            var comp = new CompetitorWithOptionsViewModel();
            //todo: remove getfullclub
            var club = await _clubService.GetFullClub(clubInitials);
            comp.BoatClassOptions = club.BoatClasses.OrderBy(c => c.Name);
            var fleets = club.Fleets.Where(f => f.FleetType == Api.Enumerations.FleetType.SelectedBoats)
                .OrderBy(f => f.Name);
            comp.FleetOptions = _mapper.Map<List<FleetSummary>>(fleets);

            var errors = _adminTipService.GetCompetitorCreateErrors(comp);
            if (errors != null && errors.Count > 0)
            {
                return View("CreateErrors", errors);
            }

            return View(comp);
        }

        // POST: Competitor/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(
            string clubInitials,
            CompetitorWithOptionsViewModel competitor,
            string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            try
            {
                var club = await _clubService.GetFullClub(clubInitials);
                if (!await _authService.CanUserEdit(User, club.Id))
                {
                    return Unauthorized();
                }
                competitor.ClubId = club.Id;
                if (!ModelState.IsValid)
                {
                    var fleets = club.Fleets.Where(f => f.FleetType == Api.Enumerations.FleetType.SelectedBoats)
                        .OrderBy(f => f.Name);
                    competitor.FleetOptions = _mapper.Map<List<FleetSummary>>(fleets);
                    return View(competitor);
                }
                if (competitor.Fleets == null)
                {
                    competitor.Fleets = new List<Fleet>();
                }
                if (competitor.FleetIds == null)
                {
                    competitor.FleetIds = new List<Guid>();
                }
                foreach (var fleetId in competitor.FleetIds)
                {
                    competitor.Fleets.Add(club.Fleets.Single(f => f.Id == fleetId));
                }
                await _competitorService.SaveAsync(competitor);
                if (!string.IsNullOrWhiteSpace(returnUrl))
                {
                    return Redirect(returnUrl);
                }
                return RedirectToAction("Index", "Admin");
            }
            catch
            {
                return View();
            }
        }

        // GET: Competitor/CreateMultiple
        public async Task<ActionResult> CreateMultiple(
            string clubInitials,
            string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            var vm = new MultipleCompetitorsWithOptionsViewModel();
            //todo: remove getfullclub
            var club = await _clubService.GetFullClub(clubInitials);
            vm.BoatClassOptions = club.BoatClasses.OrderBy(c => c.Name);
            var fleets = club.Fleets.Where(f => f.FleetType == Api.Enumerations.FleetType.SelectedBoats)
                .OrderBy(f => f.Name);
            vm.FleetOptions = _mapper.Map<List<FleetSummary>>(fleets);

            var errors = _adminTipService.GetMultipleCompetitorsCreateErrors(vm);
            if (errors != null && errors.Count > 0)
            {
                return View("CreateErrors", errors);
            }

            return View(vm);
        }

        // POST: Competitor/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> CreateMultiple(
            string clubInitials,
            MultipleCompetitorsWithOptionsViewModel competitorsVm,
            string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            try
            {
                var club = await _clubService.GetFullClub(clubInitials);
                if (!ModelState.IsValid)
                {
                    competitorsVm.BoatClassOptions = club.BoatClasses.OrderBy(c => c.Name);
                    var fleets = club.Fleets.Where(f => f.FleetType == Api.Enumerations.FleetType.SelectedBoats)
                        .OrderBy(f => f.Name);
                    competitorsVm.FleetOptions = _mapper.Map<List<FleetSummary>>(fleets);
                    return View(competitorsVm);
                }
                if (!await _authService.CanUserEdit(User, club.Id))
                {
                    return Unauthorized();
                }
                var coreCompetitors = new List<Core.Model.Competitor>();
                foreach (var comp in competitorsVm.Competitors)
                {
                    // if they didn't give a name or sail, skip this row.
                    if(String.IsNullOrWhiteSpace(comp.Name)
                        && String.IsNullOrWhiteSpace(comp.SailNumber)
                        )
                    {
                        break;
                    }
                    var currentComp = _mapper.Map<Core.Model.Competitor>(comp);
                    currentComp.ClubId = club.Id;
                    currentComp.Fleets = new List<Fleet>();
                    currentComp.BoatClassId = competitorsVm.BoatClassId;
                    
                    if (competitorsVm.FleetIds != null)
                    {
                        foreach (var fleetId in competitorsVm.FleetIds)
                        {
                            currentComp.Fleets.Add(club.Fleets.Single(f => f.Id == fleetId));
                        }
                    }
                    coreCompetitors.Add(currentComp);
                }

                foreach(var comp in coreCompetitors)
                {
                    await _competitorService.SaveAsync(comp);
                }

                if (!string.IsNullOrWhiteSpace(returnUrl))
                {
                    return Redirect(returnUrl);
                }
                return RedirectToAction("Index", "Admin");
            }
            catch
            {

                var club = await _clubService.GetFullClub(clubInitials);
                competitorsVm.BoatClassOptions = club.BoatClasses.OrderBy(c => c.Name);
                var fleets = club.Fleets.Where(f => f.FleetType == Api.Enumerations.FleetType.SelectedBoats)
                    .OrderBy(f => f.Name);
                competitorsVm.FleetOptions = _mapper.Map<List<FleetSummary>>(fleets);
                return View(competitorsVm);
            }
        }

        // GET: Competitor/Edit/5
        public async Task<ActionResult> Edit(string clubInitials, Guid id)
        {
            //todo: replace getfullclub
            var club = await _clubService.GetFullClub(clubInitials);
            if (!await _authService.CanUserEdit(User, club.Id))
            {
                return Unauthorized();
            }
            var competitor = await _competitorService.GetCompetitorAsync(id);
            if (competitor == null)
            {
                return NotFound();
            }
            if (competitor.ClubId != club.Id)
            {
                return Unauthorized();
            }
            var compWithOptions = _mapper.Map<CompetitorWithOptionsViewModel>(competitor);

            compWithOptions.BoatClassOptions = club.BoatClasses.OrderBy(c => c.Name);
            var fleets = club.Fleets.Where(f => f.FleetType == Api.Enumerations.FleetType.SelectedBoats)
                .OrderBy(f => f.Name);
            compWithOptions.FleetOptions = _mapper.Map<IList<FleetSummary>>(fleets);

            return View(compWithOptions);
        }

        // POST: Competitor/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(
            Guid id,
            CompetitorWithOptionsViewModel competitor)
        {
            try
            {
                if (!await _authService.CanUserEdit(User, competitor.ClubId))
                {
                    return Unauthorized();
                }
                var club = await _clubService.GetFullClub(competitor.ClubId);
                if (!ModelState.IsValid)
                {
                    competitor.BoatClassOptions = club.BoatClasses.OrderBy(c => c.Name);
                    var fleets = club.Fleets.Where(f => f.FleetType == Api.Enumerations.FleetType.SelectedBoats)
                        .OrderBy(f => f.Name);
                    competitor.FleetOptions = _mapper.Map<List<FleetSummary>>(fleets);
                    return View(competitor);
                }
                if (competitor.Fleets == null)
                {
                    competitor.Fleets = new List<Fleet>();
                }
                if (competitor.FleetIds == null)
                {
                    competitor.FleetIds = new List<Guid>();
                }
                foreach (var fleetId in competitor.FleetIds)
                {
                    competitor.Fleets.Add(club.Fleets.Single(f => f.Id == fleetId));
                }
                await _competitorService.SaveAsync(competitor);

                return RedirectToAction("Index", "Admin");
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
            var clubId = await _clubService.GetClubId(clubInitials);
            if (!await _authService.CanUserEdit(User, clubId))
            {
                return Unauthorized();
            }
            var competitor = await _competitorService.GetCompetitorAsync(id);
            if (competitor == null)
            {
                return NotFound();
            }
            if (competitor.ClubId != clubId)
            {
                return Unauthorized();
            }
            return View(competitor);
        }

        // POST: Competitor/Delete/5
        [HttpPost]
        [ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> PostDelete(string clubInitials, Guid id)
        {
            try
            {
                //todo: replace GetFullClub
                var club = await _clubService.GetFullClub(clubInitials);
                if (!await _authService.CanUserEdit(User, club.Id)
                    || !club.Competitors.Any(c => c.Id == id))
                {
                    return Unauthorized();
                }
                await _competitorService.DeleteCompetitorAsync(id);

                return RedirectToAction("Index", "Admin");
            }
            catch
            {
                return View();
            }
        }
    }
}