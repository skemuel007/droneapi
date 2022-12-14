using Application.Contracts.Infrastructure;
using Application.Models;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Persistence.Implementation.Audit;

namespace API.Extensions;

public static class AuditTrailExtensions
{
    public static IServiceCollection AddAuditTrail<T>(this IServiceCollection services) where T : class, IAuditTrailLog
    {
        if (services == null)
        {
            throw new ArgumentNullException(nameof(services));
        }

        return AddAuditTrail<T>(services, setupAction: null);
    }

    public static IServiceCollection AddAuditTrail<T>(this IServiceCollection services,
        Action<AuditTrailOptions> setupAction) where T : class, IAuditTrailLog
    {
        services.TryAdd(new ServiceDescriptor(typeof(IAuditTrailProvider<T>),
            typeof(AuditTrailProvider<T>), ServiceLifetime.Transient));

        if (setupAction != null)
        {
            services.Configure(setupAction);
        }

        return services;
    }
}