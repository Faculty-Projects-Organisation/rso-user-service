using Carter;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using RSO.Core.BL;
using RSO.Core.Configurations;
using RSO.Core.Repository;
using RSO.Core.UserModels;
using System.Text;
using UserServiceRSO.Repository;
using RSO.Core.Health;
using Serilog;
using NSwag.AspNetCore;

#region BUILDER
var builder = WebApplication.CreateBuilder(args);

// Register the IOptions object.
builder.Services.AddOptions<JwtSecurityTokenConfiguration>()
    .BindConfiguration("JwtSecurityTokenConfiguration");
// Explicitly register the settings objects by delegating to the IOptions object.
builder.Services.AddSingleton(resolver => resolver.GetRequiredService<IOptions<JwtSecurityTokenConfiguration>>().Value);

// Register the IOptions object.
builder.Services.AddOptions<CrossEndpointsFunctionalityConfiguration>().BindConfiguration("CrossEndpointsFunctionalityConfiguration");
// Explicitly register the settings objects by delegating to the IOptions object.
builder.Services.AddSingleton(resolver => resolver.GetRequiredService<IOptions<CrossEndpointsFunctionalityConfiguration>>().Value);

//Database settings
builder.Services.AddDbContext<UserServicesRSOContext>(options =>
options.UseNpgsql(builder.Configuration.GetConnectionString("UserServicesRSOdB")));

builder.Services.AddHealthChecks().AddCheck<DatabaseHealthCheck>("Database").AddCheck<ExternalAPICheck>("LavbicAPI");

//Lazy cache
builder.Services.AddLazyCache();
// Add unit of work & repositories.
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<IUserRepository, UserRepository>(); //In each microservice a different repository/serice is inlcuded. If more tables are needed add more repos related to the microservice.
builder.Services.AddScoped<IUserLogic, UserLogic>(); //In each microservice a different repository/serice is inlcuded.

//JWT
var jwtSecurityConfig = builder.Configuration.GetSection("JwtSecurityTokenConfiguration").Get<JwtSecurityTokenConfiguration>();
builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new()
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero,
            ValidateIssuerSigningKey = true,

            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecurityConfig.SecretKey)),
            ValidIssuer = jwtSecurityConfig.Issuer,
            ValidAudience = jwtSecurityConfig.Audience
        };
    });

//Carter
builder.Services.AddHttpContextAccessor();
builder.Services.AddCarter();
// Add services to the container.
builder.Services.AddAuthorization();
builder.Services.AddEndpointsApiExplorer();
//Open API / Swagger
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

builder.Host.UseSerilog((context, configuration) =>
    configuration.ReadFrom.Configuration(context.Configuration));
#endregion

#region APP
var app = builder.Build();
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
// Carter
app.MapCarter();
// OPENAPI SWAGGER
app.UseOpenApi();
app.UseSwaggerUi3(options =>
{
    options.Path = "/userMicroservice/openapi";
    options.TagsSorter = "Users";
});
app.UseAuthentication();
app.UseAuthorization();

app.UseSerilogRequestLogging();

app.Run();
#endregion