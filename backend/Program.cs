using System.Text.Json;
using backend.DataAccess;
using Backend.ErrorHandling;



using Microsoft.AspNetCore.Diagnostics;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddCors(options
    => options.AddDefaultPolicy(policyBuilder => policyBuilder.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod()));

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Configure PostgreSQL Database
var host = Environment.GetEnvironmentVariable("TYT_DB_HOST") ?? "localhost";
var port = Environment.GetEnvironmentVariable("TYT_DB_PORT") ?? "5432";
var frontend_host = Environment.GetEnvironmentVariable("TYT_FRONTEND_HOST") ?? "localhost";
var frontend_port = Environment.GetEnvironmentVariable("TYT_FRONTEND_PORT") ?? "5173";
var user = Environment.GetEnvironmentVariable("TYT_DB_USER") ?? "postgres";
var password = Environment.GetEnvironmentVariable("TYT_DB_PASSWORD") ?? "postgres";
var database = Environment.GetEnvironmentVariable("TYT_DB_NAME") ?? "TrackYourThings";

var connectionString = $"Host={host};Port={port};Database={database};Username={user};Password={password}";

builder.Services.AddDbContext<TytDbContext>(options => options.UseNpgsql(connectionString));
builder.Services.AddMemoryCache();
builder.Services.AddFido2(options =>
{
    options.ServerDomain = builder.Configuration["Fido2:ServerDomain"];
    options.ServerName = builder.Configuration["Fido2:ServerName"];
    options.Origins = builder.Configuration
        .GetSection("Fido2:Origins")
        .Get<HashSet<string>>();
}).AddCachedMetadataService(config =>
{
    config.AddFidoMetadataRepository();
});


// Session für temporäre Challenge-Speicherung
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(5);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

var app = builder.Build();

// Migrate database
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<TytDbContext>();
    context.Database.Migrate();
}

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();
app.MapControllers();
app.UseCors();

app.UseExceptionHandler(appBuilder =>
{
    appBuilder.Run(async context =>
    {
        var exceptionHandlerFeature = context.Features.Get<IExceptionHandlerFeature>();
        if (exceptionHandlerFeature != null)
        {
            var exception = exceptionHandlerFeature.Error;
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = exception switch
            {
                EntityNotFoundException => StatusCodes.Status404NotFound,
                InvalidOperationException or ArgumentNullException => StatusCodes.Status400BadRequest,
                _ => StatusCodes.Status500InternalServerError
            };

            Console.WriteLine(exception.Message);
            Console.WriteLine(exception.StackTrace);

            var errorResponse = new
            {
                error = exception.Message
            };

            await context.Response.WriteAsync(JsonSerializer.Serialize(errorResponse));
        }
    });
});

app.Run();
