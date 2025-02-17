﻿using SailScores.Web.Models.SailScores;
using SailScores.Web.Services.Interfaces;

namespace SailScores.Web.Services;

public class AdminTipService : IAdminTipService
{
    public void AddTips(ref AdminViewModel viewModel)
    {
        if (viewModel == null)
        {
            return;
        }
        if (viewModel.HasRaces)
        {
            return;
        }

        viewModel.Tips = new List<AdminToDoViewModel>
        {
            new()
            {
                Title = "Add location to club",
                Details = "Not required, but SailScores will get current weather automatically if a location is provided.",
                Link = new ToDoLinkViewModel
                {
                    Action = "Edit",
                    Controller = "Admin"
                },
                Completed = viewModel.Latitude.HasValue
            },
            new()
            {
                Title = "Add classes of boats",
                Details = "Even if the club sails a single type of boat, set up a class to use SailScores.",
                Link = new ToDoLinkViewModel
                {
                    Action = "Create",
                    Controller = "BoatClass"
                },
                Completed = viewModel.BoatClasses.Any()
            },
            new()
            {
                Title = "Add a season",
                Details = "Usually a year long, seasons are required. Each series will be associated with a season.",
                Link = new ToDoLinkViewModel
                {
                    Action = "Create",
                    Controller = "Season"
                },
                Completed = viewModel.Seasons.Any()
            },
            new()
            {
                Title = "Add a series",
                Details = "A group of races scored together is called a series.",
                Link = new ToDoLinkViewModel
                {
                    Action = "Create",
                    Controller = "Series"
                },
                Completed = viewModel.Series.Any()
            },
            new()
            {
                Title = "Add competitors",
                Details = "Before adding a race, set up the competitors.",
                Link = new ToDoLinkViewModel
                {
                    Action = "CreateMultiple",
                    Controller = "Competitor"
                },
                Completed = viewModel.HasCompetitors
            },

            new()
            {
                Title = "Add races",
                Details = "Enter some results in a new race.",
                Link = new ToDoLinkViewModel
                {
                    Action = "Create",
                    Controller = "Race"
                },
                Completed = viewModel.HasRaces
            },
        };

    }

    public void AddTips(ref RaceWithOptionsViewModel race)
    {
        if (race == null)
        {
            return;
        }
        if (race.Regatta == null && (race.SeriesOptions == null || race.SeriesOptions.Count == 0))
        {
            race.Tips = new List<AdminToDoViewModel> { new()
            {
                Title = "Add a series",
                Details = "If you want to score races together, add a series and a season covering this date.",
                Link = new ToDoLinkViewModel
                {
                    Action = "Create",
                    Controller = "Series"
                },
                Completed = false
            } };
        }
    }

    public IList<AdminToDoViewModel> GetRaceCreateErrors(
        RaceWithOptionsViewModel race)
    {
        var returnList = new List<AdminToDoViewModel>();
        if (race == null)
        {
            return returnList;
        }
        if (race.FleetOptions == null || race.FleetOptions.Count == 0)
        {
            returnList.Add(new AdminToDoViewModel
            {
                Title = "Add a class of boat",
                Details = "Even if the club only sails one type of boat, you need to set up a class to use SailScores. A fleet for each class will be automatically set up.",
                Link = new ToDoLinkViewModel
                {
                    Action = "Create",
                    Controller = "BoatClass"
                },
                Completed = false,
            });
        }
        if (!race.ClubHasCompetitors && (race.CompetitorOptions == null || race.CompetitorOptions.Count == 0))
        {
            returnList.Add(new AdminToDoViewModel
            {
                Title = "Add competitors",
                Details = "Before adding a race, set up the competitors.",
                Link = new ToDoLinkViewModel
                {
                    Action = "CreateMultiple",
                    Controller = "Competitor"
                },
                Completed = false
            });
        }
        if ((returnList?.Count ?? 0) >= 1
            && race.Regatta == null
            && (race.SeriesOptions == null || race.SeriesOptions.Count == 0))
        {
            returnList.Add(new AdminToDoViewModel
            {
                Title = "Add a series",
                Details = "If you want to score races together, add a series.",
                Link = new ToDoLinkViewModel
                {
                    Action = "Create",
                    Controller = "Series"
                },
                Completed = false
            });
        }
        return returnList;
    }

    public IList<AdminToDoViewModel> GetSeriesCreateErrors(SeriesWithOptionsViewModel series)
    {
        var returnList = new List<AdminToDoViewModel>();

        if (series == null)
        {
            return returnList;
        }
        if (series.SeasonOptions == null || !series.SeasonOptions.Any())
        {
            returnList.Add(new AdminToDoViewModel
            {
                Title = "Add a season",
                Details = "Before creating a series you need to set up a season.",
                Link = new ToDoLinkViewModel
                {
                    Action = "Create",
                    Controller = "Season"
                },
                Completed = false
            });
        }

        return returnList;
    }


    public IList<AdminToDoViewModel> GetCompetitorCreateErrors(
        CompetitorWithOptionsViewModel competitor)
    {
        var returnList = new List<AdminToDoViewModel>();
        if (competitor == null)
        {
            return returnList;
        }
        if (competitor.BoatClassOptions == null
            || !competitor.BoatClassOptions.Any())
        {
            returnList.Add(new AdminToDoViewModel
            {
                Title = "Add a class",
                Details = "Before creating a competitor, add the types of boats that are sailed by your club.",
                Link = new ToDoLinkViewModel
                {
                    Action = "Create",
                    Controller = "BoatClass"
                },
                Completed = false
            });
        }

        return returnList;
    }
    public IList<AdminToDoViewModel> GetMultipleCompetitorsCreateErrors(
        MultipleCompetitorsWithOptionsViewModel vm)
    {
        var returnList = new List<AdminToDoViewModel>();
        if (vm == null)
        {
            return returnList;
        }
        if (vm.BoatClassOptions == null
            || !vm.BoatClassOptions.Any())
        {
            returnList.Add(new AdminToDoViewModel
            {
                Title = "Add a class",
                Details = "Before creating a competitor, add the types of boats that are sailed by your club.",
                Link = new ToDoLinkViewModel
                {
                    Action = "Create",
                    Controller = "BoatClass"
                },
                Completed = false
            });
        }

        return returnList;
    }
}