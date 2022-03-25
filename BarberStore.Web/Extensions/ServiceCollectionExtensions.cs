﻿using BarberStore.Core.Contracts;
using BarberStore.Core.Services;
using BarberStore.Infrastructure.Data;
using BarberStore.Infrastructure.Data.Models;
using BarberStore.Infrastructure.Data.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace BarberStore.Web.Extensions
{
    public static class ServiceCollectionExtension
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration config)
        {
            services
                .AddScoped<IApplicationDbRepository, ApplicationDbRepository>()
                .AddScoped<IStoreService, StoreService>()
                .AddScoped<IAppointmentService, AppointmentService>()
                .AddScoped<IArticleService, ArticleService>();

            services.AddAuthentication()
                .AddFacebook(options =>
                {
                    options.ClientId = config["Authentication:Facebook:ClientId"];
                    options.ClientSecret = config["Authentication:Facebook:ClientSecret"];
                })
                .AddGoogle(options =>
                {
                    options.ClientId = config["Authentication:Google:ClientId"];
                    options.ClientSecret = config["Authentication:Google:ClientSecret"];
                });
            return services;
        }

        public static IServiceCollection AddApplicationIdentity(this IServiceCollection services)
        {
            services
                .AddDefaultIdentity<ApplicationUser>(options =>
                    options.SignIn.RequireConfirmedAccount = true)
                .AddRoles<IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>();
            return services;
        }

        public static IServiceCollection AddApplicationDbContexts(this IServiceCollection services, IConfiguration config)
        {
            var connectionString = config.GetConnectionString("DefaultConnection");
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(connectionString));
            services.AddDatabaseDeveloperPageExceptionFilter();

            return services;
        }
    }
}
