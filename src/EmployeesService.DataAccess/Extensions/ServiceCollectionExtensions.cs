using EmployeesService.Core.Contracts.Repositories;
using EmployeesService.DataAccess.Configurations;
using EmployeesService.DataAccess.DbContexts;
using EmployeesService.DataAccess.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Microsoft.Extensions.DependencyInjection
{
    /// <summary>
    /// Class with extensions for <see cref="IServiceCollection" />
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddEmployeesServiceDb(this IServiceCollection services,
            IConfiguration configuration)
        {
            services.Configure<DbConfiguration>(it =>
                configuration.GetSection(nameof(DbConfiguration)).Get<DbConfiguration>());
            var dbConfiguration = configuration.GetSection(nameof(DbConfiguration)).Get<DbConfiguration>();

            services.AddDbContext<EmployeesDbContext>(opt =>
            {
                opt.UseNpgsql(dbConfiguration.ConnectionString);
            });

            return services;
        }

        public static IServiceCollection AddEmployeesRepositories(this IServiceCollection services)
        {
            services.AddScoped<IEmployeeRepository, EmployeeRepository>();
            services.AddScoped<IConferenceRepository, ConferenceRepository>();

            return services;
        }
    }
}
