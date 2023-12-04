using Carter;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
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
                                                               // Logic
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

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(swagger =>
{
    swagger.SwaggerDoc("v1", new OpenApiInfo()
    {
        Description = "User microservice for E-commerce app.",
        Title = "RSO project.",
        Version = "v1",
        Contact = new OpenApiContact()
        {
            Name = "Aleksander Kovac & Urban Poljsak",
            Url = new Uri("https://www.youtube.com/watch?v=dQw4w9WgXcQ")
        }
    });
    swagger.AddSecurityDefinition("jwt_auth", new OpenApiSecurityScheme()
    {
        Description = "Basic authorization with JWT token.",
        Name = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey
    });
    swagger.AddSecurityRequirement(new OpenApiSecurityRequirement()
            {
                {new OpenApiSecurityScheme()
                {
                    Reference = new OpenApiReference()
                    {
                        Id="jwt_auth",
                        Type = ReferenceType.SecurityScheme
                    }
                },Array.Empty<string>() },
            });
});

// APP.
var app = builder.Build();

// Carter
app.MapCarter();

app.UseSwagger();
app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "API"));

app.UseAuthentication();
app.UseAuthorization();

app.Run();
