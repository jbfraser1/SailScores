﻿using Ganss.XSS;
using Microsoft.Extensions.DependencyInjection;
using SailScores.Web.Services;
using SailScores.Web.Services.Interfaces;

namespace SailScores.Web.Extensions
{
    public static class DependencyInjectionExtensions
    {
        public static void RegisterWebSailScoresServices(
            this IServiceCollection services)
        {
            services.AddScoped<IClubService, ClubService>();
            services.AddScoped<ISeriesService, SeriesService>();
            services.AddScoped<IFleetService, FleetService>();
            services.AddScoped<IRaceService, RaceService>();
            services.AddScoped<ICompetitorService, CompetitorService>();
            services.AddScoped<IAuthorizationService, AuthorizationService>();
            services.AddScoped<IBoatClassService, BoatClassService>();
            services.AddScoped<ISeasonService, SeasonService>();
            services.AddScoped<IAnnouncementService, AnnouncementService>();
            services.AddScoped<IRegattaService, RegattaService>();
            services.AddScoped<IAdminTipService, AdminTipService>();
            services.AddScoped<ICsvService, CsvService>();
            services.AddScoped<IMergeService, MergeService>();
            services.AddScoped<IClubRequestService, ClubRequestService>();
            services.AddScoped<IWeatherService, WeatherService>();
            services.AddScoped<IAdminService, AdminService>();
            services.AddScoped<ITemplateHelper, TemplateHelper>();
            services.AddScoped<IPermissionService, PermissionService>();
            services.AddScoped<ISpeechService, SpeechService>();
            services.AddSingleton<IHtmlSanitizer>(new HtmlSanitizer());

            services.AddSingleton<AppVersionInfo>();
            services.AddSingleton<AppSettingsService>();

        }
    }
}
