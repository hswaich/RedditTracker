﻿using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RedditTracker.Repository;
using RedditTracker.Services;

namespace RedditTracker
{
    public static class Startup
    {
        public static IServiceCollection ConfigureServices()
        {
            var services = new ServiceCollection();

            var builder = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json", optional: true, reloadOnChange: false);

            IConfiguration configuration = builder.Build();
            services.AddHttpClient();
            services.AddSingleton(configuration);
            services.AddSingleton<IHttpClientWrapper, HttpClientWrapper>();
            services.AddSingleton<IUtilityService, UtilityService>();
            services.AddSingleton<IStatisticsService, StatisticsService>();
            services.AddSingleton<IApiService, ApiService>();
            services.AddSingleton<IWatcherService, WatcherService>();
            services.AddSingleton<IDatabaseRepository, DatabaseRepository>();

            return services;
        }
    }
}