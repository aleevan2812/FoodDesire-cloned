﻿using FoodDesire.IMS.Core.Services;
using Microsoft.Extensions.DependencyInjection;

namespace FoodDesire.IMS.Core;
public static class Configure {
    public static void ConfigureServices(IServiceCollection services, string connectionString) {
        DAL.Configure.ConfigureServices(services, connectionString);
        FoodDesire.Core.Configure.ConfigureServices(services);

        services.AddSingleton<IFileService, FileService>();

        services.AddScoped<IHomeService, HomeService>();
    }
}
