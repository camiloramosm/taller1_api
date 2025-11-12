using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Supabase;
using TodoList.Application.Interfaces;
using TodoList.Application.Services;
using TodoList.Infrastructure.Repositories;

namespace TodoList.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // Configuración de Supabase
        var supabaseUrl = configuration["Supabase:Url"] 
            ?? throw new InvalidOperationException("Supabase:Url not found in configuration.");
        
        var supabaseKey = configuration["Supabase:Key"] 
            ?? throw new InvalidOperationException("Supabase:Key not found in configuration.");

        // Registrar cliente de Supabase
        services.AddScoped<Client>(_ => new Client(
            supabaseUrl,
            supabaseKey,
            new SupabaseOptions
            {
                AutoRefreshToken = true,
                AutoConnectRealtime = false
            }
        ));

        // Registrar repositorio y servicio
        services.AddScoped<ITodoRepository, SupabaseTodoRepository>();
        services.AddScoped<ITodoService, TodoService>();

        return services;
    }
}

