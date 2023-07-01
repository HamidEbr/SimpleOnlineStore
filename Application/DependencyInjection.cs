using Application.Services;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services, Type cachingBehavior, Type validationBehavior, Type startupType)
    {
        services.AddScoped<IOrderService, OrderService>();
        services.AddScoped<IProductService, ProductService>();
        services.AddValidatorsFromAssembly(startupType.Assembly, includeInternalTypes: true);
        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(startupType.Assembly);
            cfg.RegisterServicesFromAssembly(typeof(DependencyInjection).Assembly);
            cfg.AddOpenBehavior(cachingBehavior);
            cfg.AddOpenBehavior(validationBehavior);
        });
        return services;
    }
}