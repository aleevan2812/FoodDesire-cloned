﻿using AutoMapper;
using FoodDesire.IMS.Activation;
using FoodDesire.IMS.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace FoodDesire.IMS.Helpers;
internal static class AppConfigurator {
    internal static void Configure(HostBuilderContext context, IConfigurationBuilder config) {
        string environmentName = context.HostingEnvironment.EnvironmentName;
        AppSettings.Configure.ConfigureEnvironment(config, environmentName);
    }

    internal static void Configure(HostBuilderContext context, IServiceCollection services) {
        // Default Activation Handler
        services.AddTransient<ActivationHandler<LaunchActivatedEventArgs>, DefaultActivationHandler>();

        // Other Activation Handlers

        // Core Services
        string connectionString = context.Configuration.GetConnectionString("DefaultConnection")!;
        Core.Configure.ConfigureServices(services, connectionString);

        // Services
        services.AddSingleton<ILocalSettingsService, LocalSettingsService>();
        services.AddSingleton<IThemeSelectorService, ThemeSelectorService>();
        services.AddTransient<INavigationViewService, NavigationViewService>();

        services.AddSingleton<IActivationService, ActivationService>();
        services.AddSingleton<IPageService, PageService>();
        services.AddSingleton<INavigationService, NavigationService>();

        // AutoMapper
        MapperConfiguration? configuration = new(DtoConfigurator.Configure);
        IMapper? mapper = configuration.CreateMapper();
        services.AddSingleton(mapper);

        // Views and ViewModels
        services.AddTransient<SettingsViewModel>();
        services.AddTransient<SettingsPage>();
        services.AddTransient<HomeViewModel>();
        services.AddTransient<HomePage>();
        services.AddTransient<IngredientsViewModel>();
        services.AddTransient<IngredientsPage>();
        services.AddTransient<ShellViewModel>();
        services.AddTransient<ShellPage>();

        // Configuration
        services.Configure<LocalSettingsOptions>(context.Configuration.GetSection(nameof(LocalSettingsOptions)));
    }


}
