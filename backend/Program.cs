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

builder.Services.AddCors(opt => opt.AddPolicy("frontend", policy =>
    policy
        .WithOrigins("http://localhost:5173")
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


// Session für temporäre Challenge-Speicherung und Sessions
builder.Services.AddDistributedMemoryCache();
builder.Services.AddScoped<IPasskeyService, PasskeyService>();
builder.Services.AddScoped<ITrackedItemService, TrackedItemService>();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(opt =>
    {
        opt.TokenValidationParameters = new()
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Secret"]!))
        };
    });

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
