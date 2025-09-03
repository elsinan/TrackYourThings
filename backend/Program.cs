using System;
using System.IO;
using System.Linq;
using System.Text.Json;
using backend.DataAccess;
using backend.ErrorHandling;
using backend.Swagger;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;


var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddCors(options
    => options.AddDefaultPolicy(policyBuilder => policyBuilder.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod()));

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


builder.Services.AddDataAccess();

var app = builder.Build();

app.Services.MigrateDatabase();

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
