using System.Text;
using System.Text.Json;
using backend.Auth;
using backend.DataAccess;
using backend.TrackedItems;
using Backend.ErrorHandling;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

var frontend_host = Environment.GetEnvironmentVariable("TYT_ORIGINS") ?? "http://localhost:5173";
var frontend_domain = Environment.GetEnvironmentVariable("TYT_FRONTEND_DOMAIN") ?? "localhost";

builder.Services.AddCors(opt => opt.AddPolicy("frontend", policy =>
    policy
        .WithOrigins(frontend_host)
        .AllowAnyHeader()
        .AllowAnyMethod()
        .AllowCredentials()
));
builder.Services.ConfigureApplicationCookie(options => {
    options.Cookie.SameSite = SameSiteMode.None;
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Configure PostgreSQL Database
var host = Environment.GetEnvironmentVariable("TYT_DB_HOST") ?? "localhost";
var port = Environment.GetEnvironmentVariable("TYT_DB_PORT") ?? "5432";
var database = Environment.GetEnvironmentVariable("TYT_DB_NAME") ?? "TrackYourThings";
var user = Environment.GetEnvironmentVariable("TYT_DB_USER") ?? "postgres";
var password = Environment.GetEnvironmentVariable("TYT_DB_PASSWORD") ?? "postgres";

var connectionString = $"Host={host};Port={port};Database={database};Username={user};Password={password}";

builder.Services.AddDbContext<TytDbContext>(options => options.UseNpgsql(connectionString));

builder.Services.AddMemoryCache();
builder.Services.AddFido2(options =>
{
    options.ServerDomain = frontend_domain;
    options.ServerName = "TrackYourThings";
    options.Origins = new HashSet<string> { $"{frontend_host}" };
}).AddCachedMetadataService(config =>
{
    config.AddFidoMetadataRepository();
});


// Session für temporäre Challenge-Speicherung und Sessions
builder.Services.AddDistributedMemoryCache();
builder.Services.AddScoped<IPasskeyService, PasskeyService>();
builder.Services.AddScoped<ITrackedItemService, TrackedItemService>();


builder.Services.AddSession(options =>
{
    options.Cookie.SameSite = SameSiteMode.None; // Erlaubt Cross-Site Cookies
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always; // Benötigt HTTPS
    options.IdleTimeout = TimeSpan.FromMinutes(2); // Kurz für Passkeys reicht
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
app.UseCors("frontend");
app.UseSession();

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
