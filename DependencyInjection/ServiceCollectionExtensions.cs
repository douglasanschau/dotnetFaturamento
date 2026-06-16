using Microsoft.Extensions.DependencyInjection;
using faturamentoClienteDotnet.Services;
using faturamentoClienteDotnet.Data;

namespace faturamentoClienteDotnet.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        // Services
        services.AddScoped<JwtService>();
        services.AddScoped<UsuarioService>();
        services.AddScoped<ClienteService>();
        services.AddScoped<FaturamentoService>();

        // Data
        services.AddScoped<Database>();

        return services;
    }
}