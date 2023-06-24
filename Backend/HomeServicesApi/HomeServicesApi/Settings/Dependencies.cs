using DataLayer.UnitOfWork;
using DataLayer;
using Microsoft.AspNetCore.Authorization;
using Core.Services;
using DataLayer.Repositories;

namespace HomeServicesApi.Settings
{
    public static class Dependencies
    {
        public static void Inject(WebApplicationBuilder applicationBuilder)
        {
            applicationBuilder.Services.AddControllers();
            applicationBuilder.Services.AddSwaggerGen();

            applicationBuilder.Services.AddDbContext<AppDbContext>();

            AddRepositories(applicationBuilder.Services);
            AddServices(applicationBuilder.Services);
        }

        private static void AddServices(IServiceCollection services)
        {
            services.AddScoped<CustomersService>();
            services.AddScoped<ProvidersService>();
            services.AddScoped<AdminsService>();
            services.AddScoped<AuthenticationService>();
            services.AddScoped<AuthorizationService>();
        }

        private static void AddRepositories(IServiceCollection services)
        {
            services.AddScoped<IUnitOfWork, UnitOfWork>();
        }

    }
}
