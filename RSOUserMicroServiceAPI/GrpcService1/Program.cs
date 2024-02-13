using GrpcService1.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using RSO.Core.BL;
using RSO.Core.Configurations;
using RSO.Core.Repository;
using RSO.Core.UserModels;
using UserServiceRSO.Repository;

namespace GrpcService1
{
    public class Program
    {
        public static void Main(string[] args)
        {
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

            builder.Services.AddDbContext<UserServicesRSOContext>(options =>
options.UseNpgsql(builder.Configuration.GetConnectionString("UserServicesRSOdB")));

            //JWT
            var jwtSecurityConfig = builder.Configuration.GetSection("JwtSecurityTokenConfiguration").Get<JwtSecurityTokenConfiguration>();


            builder.Services.AddLazyCache();
            // Add services to the container.
            builder.Services.AddGrpc();
            builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
            builder.Services.AddScoped<IUserRepository, UserRepository>(); //In each microservice a different repository/serice is inlcuded. If more tables are needed add more repos related to the microservice.
            builder.Services.AddScoped<IUserLogic, UserLogic>(); //In each microservice a different repository/serice is inlcuded.
            var app = builder.Build();

            app.MapGrpcService<GreeterService>();
            app.MapGet("/", () => "Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");
            app.Run();
        }
    }
}