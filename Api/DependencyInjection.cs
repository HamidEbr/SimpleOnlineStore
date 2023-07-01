using Api.Behaviors;
using FluentValidation;

namespace Api;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services, IConfiguration configuration, Type startupType)
    {
        services.AddValidatorsFromAssembly(startupType.Assembly, includeInternalTypes: true);
        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(startupType.Assembly);
            cfg.RegisterServicesFromAssembly(typeof(DependencyInjection).Assembly);

            cfg.AddOpenBehavior(typeof(ValidationBehavior<,>));
        });
        return services;
    }
}