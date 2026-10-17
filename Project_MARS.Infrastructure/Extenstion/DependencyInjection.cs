using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection; 
using Project_MARS.Application.Interfaces;
using Project_MARS.Application.Services;
using Project_MARS.Infrastructure.Data;
using Project_MARS.Infrastructure.Repositories;
using Project_MARS.Infrastructure.Security;

namespace Project_MARS.Infrastructure.Extenstion
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfigurationManager configuration)
        {
            services.AddDbContext<AppDbContext>(options =>
            {
                options.UseSqlServer(configuration.GetConnectionString("Default"));
            });

            services.Configure<JwtOptions>(configuration.GetSection("Jwt"));


            services.AddScoped<IAutoServiceRepository, AutoServiceRepository>();
            services.AddScoped<IRepairBoxRepository, RepairBoxRepository>();
            services.AddScoped<IReservationRepository, ReservationRepository>();
            services.AddScoped<ITimeFlagRepository, TimeFlagRepository>();
            services.AddScoped<IServiceCategoryRepository, ServiceCategoryRepository>();
            services.AddScoped<IWorkScheduleRepository, WorkScheduleRepository>();
            services.AddScoped<IAutoServiceBossRepository, AutoServiceBossRepository>();
            services.AddScoped<IRepairBoxAssignmentRepository, RepairBoxAssignmentRepository>();
            services.AddScoped<ICarRepository, CarRepository>();
            services.AddScoped<IRateRepository, RateRepository>();
            services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
            services.AddScoped<IAuthService, AuthService>();

            services.AddSingleton<ITokenGenerator, TokenGenerator>();

            services.AddHttpClient<IVinDecoderService, VinDecoderService>();


            return services;
        }
    }
}
