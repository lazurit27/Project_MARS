using Microsoft.Extensions.DependencyInjection;
using Project_MARS.Application.Services;

namespace Project_MARS.Application.Extenstion
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            services.AddScoped<IAdminService, AdminService>();
            services.AddScoped<IAutoServiceBossService, AutoServiceBossService>();
            services.AddScoped<IAutoServiceService, AutoServiceService>();
            services.AddScoped<ICarService, CarService>();
            services.AddScoped<IRateService, RateService>();
            services.AddScoped<IRepairBoxAssignmentService, RepairBoxAssignmentService>();
            services.AddScoped<IRepairBoxService, RepairBoxService>();
            services.AddScoped<IReservationService, ReservationService>();
            services.AddScoped<ITimeFlagService, TimeFlagService>();
            services.AddScoped<IWorkScheduleService, WorkScheduleService>();

            return services;
        }
    }

}
