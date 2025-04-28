using System.Reflection;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Scalar.AspNetCore;
using MovieAPI.MovieEPI.Endpoints;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Logging;
using Microsoft.IdentityModel.Tokens;
using MovieAPI.Context;
using MovieAPI.Services;

var builder = WebApplication.CreateBuilder(args);

IdentityModelEventSource.ShowPII = true;

builder.Services.AddControllers();

builder.Services.AddScoped<AuthHelper>();

builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<ICommentService, CommentService>();
builder.Services.AddScoped<IReviewService, ReviewService>();

builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseNpgsql(Environment.GetEnvironmentVariable("CONNECTION_STRING"));
});


builder.Services.AddOpenApi();



builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!)),
            ValidateIssuerSigningKey = true,
        };

        options.Events = new JwtBearerEvents
        {
            OnAuthenticationFailed = context =>
            {
                Console.WriteLine("Authentication failed: " + context.Exception);
                return Task.CompletedTask;
            }
        };
    });

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly", policy => policy.RequireClaim(ClaimTypes.Role, "1"));
});

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

app.UseCors(options => options.SetIsOriginAllowedToAllowWildcardSubdomains()
    .AllowAnyOrigin()
    .AllowAnyMethod()
    .AllowAnyHeader());


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



app.UseStaticFiles();

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
