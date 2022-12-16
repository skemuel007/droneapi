using Application.Contracts.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Persistence.Repositories;

namespace Persistence;

public static class PersistenceServiceRegistration
{
    public static IServiceCollection AddPersistenceServices(this IServiceCollection services,
        IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DronesAppConnectionString");
        services.AddDbContext<DronesAppContext>(options =>
            options.UseSqlServer(connectionString));
        
        #region -- Add Repository to service collection

        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
        services.AddScoped<IDronesRepository, DroneRepository>();
        services.AddScoped<IMedicationRepository, MedicationRepository>();
        services.AddScoped<IDroneRequestRepository, DroneRequestRepository>();
        services.AddScoped<IDronePayloadRepository, DronePayloadRepository>();
        
        #endregion

        return services;
    }
}