using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services, Type validationBehavior, Type startupType)
    {
        services.AddValidatorsFromAssembly(startupType.Assembly, includeInternalTypes: true);
        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(startupType.Assembly);
            cfg.RegisterServicesFromAssembly(typeof(DependencyInjection).Assembly);

            cfg.AddOpenBehavior(validationBehavior);
        });
        return services;
    }
}