using Carter;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using NSwag;
using RSO.Core.BL;
using RSO.Core.Configurations;
using RSO.Core.UserModels;
using RSO.Core.Repository;
using System.Text;
using UserServiceRSO.Repository;

var builder = WebApplication.CreateBuilder(args);

// Register the IOptions object.
builder.Services.AddOptions<UserServicesSettingsConfiguration>()
    .BindConfiguration("UserServicesSettingsConfiguration");
// Explicitly register the settings objects by delegating to the IOptions object.
builder.Services.AddSingleton(resolver => resolver.GetRequiredService<IOptions<UserServicesSettingsConfiguration>>().Value);

// Register the IOptions object.
builder.Services.AddOptions<JwtSecurityTokenConfiguration>()
    .BindConfiguration("JwtSecurityTokenConfiguration");
// Explicitly register the settings objects by delegating to the IOptions object.
builder.Services.AddSingleton(resolver => resolver.GetRequiredService<IOptions<JwtSecurityTokenConfiguration>>().Value);

//Database settings
builder.Services.AddDbContext<UserServicesRSOContext>(options =>
options.UseNpgsql(builder.Configuration.GetConnectionString("UserServicesRSOdB")));

//Lazy cache
builder.Services.AddLazyCache();
// Add unit of work & repositories.
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<IUserRepository, UserRepository>(); //In each microservice a different repository/serice is inlcuded. If more tables are needed add more repos related to the microservice.
builder.Services.AddScoped<IUserLogic, UserLogic>(); //In each microservice a different repository/serice is inlcuded.

//JWT
builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(o => o.TokenValidationParameters = new()
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        // Read from appsettings.json ...WARNING VALUES MUST BE THE SAME IN ALL IMPLEMENTATIONS.

        // BETTER, CLEANER WAY?
        IssuerSigningKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(builder.Configuration
            .GetSection("JwtSecurityTokenConfiguration").
            Get<JwtSecurityTokenConfiguration>().SecretKey)),
        ValidIssuer = builder.Configuration
            .GetSection("JwtSecurityTokenConfiguration").
            Get<JwtSecurityTokenConfiguration>().Issuer,
        ValidAudience = builder.Configuration
            .GetSection("JwtSecurityTokenConfiguration").
            Get<JwtSecurityTokenConfiguration>().Audience
    });
//Carter
builder.Services.AddHttpContextAccessor();
builder.Services.AddCarter();
// Add services to the container.
builder.Services.AddAuthorization();
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddOpenApiDocument(options =>
{
    options.PostProcess = document =>
    {
        document.Info = new()
        {
            Version = "v1",
            Title = "User microservices API",
            Description = "User microservices API endpoints",
            TermsOfService = "Lol.",
            Contact = new()
            {
                Name = "Aleksander Kovac & Urban Poljsak",
                Url = "https://www.youtube.com/watch?v=dQw4w9WgXcQ"
            }
        };
    };
    options.UseControllerSummaryAsTagDescription = true;
});
// APP.
var app = builder.Build();
// Carter
app.MapCarter();
app.UseOpenApi();
app.UseSwaggerUi3(options =>
{
    options.Path = "/openapi";
    options.TagsSorter = "Users";
});
app.UseAuthentication();
app.UseAuthorization();

app.Run();
