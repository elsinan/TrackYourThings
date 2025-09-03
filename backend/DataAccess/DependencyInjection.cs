using System;
using System.IO;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace backend.DataAccess;

/// <summary>
/// Extension methods for dependency injection.
/// </summary>
public static class DependencyInjection
{
    /// <summary>
    /// Adds the necessary data access services to the service collection.
    /// </summary>
    public static IServiceCollection AddDataAccess(this IServiceCollection serviceCollection)
    {
        var url = Environment.GetEnvironmentVariable("TYT_DB_URL");
        var port = Environment.GetEnvironmentVariable("TYT_DB_PORT");
        var user = GetEnvVarOrLoadFromFile("TYT_DB_USER");
        var password = GetEnvVarOrLoadFromFile("TYT_DB_PASSWORD");
        var database = GetEnvVarOrLoadFromFile("TYT_DB_NAME");

        var connectionString = $"Host={url};Port={port};User Id={user};Password={password};Database={database}";

        serviceCollection.AddDbContext<TytDbContext>(options => options.UseSqlServer(connectionString));

        return serviceCollection;
    }

    /// <summary>
    /// Migrates the database.
    /// </summary>
    public static void MigrateDatabase(this IServiceProvider serviceProvider)
    {
        using var serviceScope = serviceProvider.CreateScope();
        var services = serviceScope.ServiceProvider;

        var dbContext = services.GetRequiredService<TytDbContext>();
        dbContext.Database.Migrate();
    }

    private static string GetEnvVarOrLoadFromFile(string envVarName)
    {
        var value = Environment.GetEnvironmentVariable(envVarName);

        if (value is not null)
        {
            return value;
        }

        var path = Environment.GetEnvironmentVariable(envVarName + "_FILE")
                   ?? throw new InvalidOperationException(
                       $"Either {envVarName} or {envVarName}_FILE must be configured");

        return File.ReadAllText(path);
    }
}