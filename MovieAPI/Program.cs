using System.Reflection;
using Scalar.AspNetCore;
using MovieAPI.MovieEPI.Endpoints;
using Microsoft.EntityFrameworkCore;
using MovieAPI.Context;
using MovieAPI.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseNpgsql(Environment.GetEnvironmentVariable("CONNECTION_STRING"));
});
builder.Services.AddOpenApi();
//builder.Services.AddScoped<MovieService>();
//builder.Services.AddScoped<CategoryService>();
//builder.Services.AddScoped<GenresService>();
//builder.Services.AddScoped<DirectorService>();
var servicesAssembly = Assembly.GetExecutingAssembly();
var serviceTypes = servicesAssembly.GetTypes()
    .Where(t => t.IsClass && t.Namespace == "MovieAPI.Services");

foreach (var serviceType in serviceTypes)
{
    builder.Services.AddScoped(serviceType);
}


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwaggerUI(option =>
    {
        option.SwaggerEndpoint("/openapi/v1.json", "OpenApi V1");
    });
    app.MapScalarApiReference();

}

app.MapMovieEndpoints();
app.MapDirectorEndpoint();
app.MapGenreEndpoint();
app.MapCategoryEndpoint();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
