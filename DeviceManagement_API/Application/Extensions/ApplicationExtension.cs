using Application.Abstraction;
using Application.Abstraction.CQRS;
using Microsoft.Extensions.DependencyInjection;

namespace Application.Extensions;

public static class ServiceCollectionExtension
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        var assembly = typeof(ICommandHandler<>).Assembly;

        services.Scan(scan => scan
            .FromAssemblies(assembly)
            .AddClasses(classes => classes.AssignableToAny(
                typeof(ICommandHandler<>),
                typeof(ICommandHandler<,>),
                typeof(IQueryHandler<,>)
            ))
            .AsImplementedInterfaces()
            .AsSelf()
            .WithScopedLifetime()
        );
        services.AddScoped<IMediator>(sp => new Mediator(assembly, sp));



        return services;
    }
}
