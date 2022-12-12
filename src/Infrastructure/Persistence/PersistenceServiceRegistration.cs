using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Persistence;

public static class PersistenceServiceRegistration
{
    public static IServiceCollection AddPersistenceServices(this IServiceCollection services,
        IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DronesAppConnectionString");
        services.AddDbContext<DronesAppContext>(options =>
            options.UseSqlServer(connectionString));

        return services;
    }
}