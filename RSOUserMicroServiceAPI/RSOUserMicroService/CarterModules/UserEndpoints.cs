using Carter;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using RSO.Core.AdModels;
using RSO.Core.BL;
using RSO.Core.BL.LogicModels;
using RSO.Core.UserModels;

namespace UserServiceRSO.CarterModules;

public class UserEndpoints : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapHealthChecks("/users/api/health");

        app.MapGet("/users/hello", (HttpContext httpContext) => httpContext.Response.WriteAsync("Hello from user service."));

        app.MapGet("/hello", (HttpContext httpContext) => httpContext.Response.WriteAsync("Hello from user service."));

        // Group for /users/api endpoinds.
        var group = app.MapGroup("/users/api/");

        // Login and register options.
        group.MapPost("/login", Login).WithName(nameof(Login)).
            Produces(StatusCodes.Status200OK).WithDisplayName("Lol").
            Produces(StatusCodes.Status400BadRequest).
            AllowAnonymous().WithTags("Users");

        group.MapPost("/register", Register).WithName(nameof(Register)).
            Produces(StatusCodes.Status201Created).
            Produces(StatusCodes.Status400BadRequest).
            Produces(StatusCodes.Status404NotFound).WithDescription("Ne najdem").
            AllowAnonymous().WithTags("Users");


        // Methods for  /users/api/id endpoints.
        group.MapGet("/", GetAllUsers).WithName(nameof(GetAllUsers)).
        Produces(StatusCodes.Status200OK).WithTags("Users");

        group.MapGet("{id}", GetUserById).WithName(nameof(GetUserById)).
            Produces(StatusCodes.Status200OK).
            Produces(StatusCodes.Status400BadRequest).WithSummary("Bad request").
            Produces(StatusCodes.Status401Unauthorized).WithTags("Users").WithDescription("Gets the user, specified by the id.");

        //JWT
        group.MapDelete("{id}", DeleteUserById).WithName(nameof(DeleteUserById)).
            Produces(StatusCodes.Status204NoContent).
            Produces(StatusCodes.Status400BadRequest).
            Produces(StatusCodes.Status401Unauthorized).
            WithTags("Users").
            WithDescription("Deletes the user specified by the user ID");

        //JWT
        group.MapPatch("{id}", UpdateUserNameById).WithName(nameof(UpdateUserNameById)).
            Produces(StatusCodes.Status204NoContent).
            Produces(StatusCodes.Status400BadRequest).
            Produces(StatusCodes.Status401Unauthorized).
            WithTags("Users").
            WithDescription("Updates the user name of the user specified by the user ID");

        group.MapPatch("/update", UpdateUser).WithName(nameof(UpdateUser)).
             Produces(StatusCodes.Status200OK)
            .WithDescription("Ok response.")
            .Produces(StatusCodes.Status400BadRequest)
            .WithDescription("Validation error")
            .Produces(StatusCodes.Status409Conflict)
            .WithDescription("More than one user exist with this email or username.")
            .WithTags("Users");

        //group.MapGet("/health", HealthCheck).WithName(nameof(HealthCheck)).
        //   Produces(StatusCodes.Status200OK).WithTags("Health");
    }

    /// <summary>
    /// Performs login and gets the JWT token.
    /// </summary>
    /// <param name="loginCredentials">The login credentials.</param>
    /// <param name="userLogic"><see cref="IUserLogic"/> dependency injection.</param>
    /// <returns>A JWT token as a string.</returns>
    public static async Task<Results<Ok<string>, BadRequest<string>>> Login([FromBody] LoginCredentials loginCredentials, IUserLogic userLogic, Serilog.ILogger logger, HttpContext httpContext)
    {
        var requestId = httpContext?.TraceIdentifier ?? "Unknown";
        logger.Information("user-service: Login method called. RequestID: {@requestId}", requestId);

        if (string.IsNullOrEmpty(loginCredentials.EmailorUsername) || string.IsNullOrEmpty(loginCredentials.Password))
        {
            return TypedResults.BadRequest("Username (or email) and password cannot be empty.");
        }
        else
        {
            var user = await userLogic.GetUserByUsernameOrEmailAndPasswordAsync(loginCredentials.EmailorUsername, loginCredentials.Password);
            var jwt = userLogic.GetJwtToken(user);
            return jwt is null
                ? TypedResults.BadRequest("The user with the specified username/email and password doesn't exist.")
                : TypedResults.Ok(jwt);
        }
    }

    /// <summary>
    /// Performs registration and gets the JWT token.
    /// </summary>
    /// <param name="newUser">Data for the new user that is going to be created.</param>
    /// <param name="userLogic">DI for B(usiness) L(logic) layer.</param>
    /// <returns>A JWT token as a string.</returns>
    public static async Task<Results<Created<string>, NotFound<string>, BadRequest<string>, Conflict<string>>> Register(User newUser, IUserLogic userLogic, Serilog.ILogger logger, HttpContext httpContext)
    {
        var requestId = httpContext?.TraceIdentifier ?? "Unknown";
        logger.Information("user-service: Register method called. RequestID: {@requestId}", requestId);
        //Use FLUENT_VALIDATION LIB NEXT TIME.

        if (string.IsNullOrEmpty(newUser.UserName))
            return TypedResults.BadRequest("User name or email cannot be empty.");
        if (await userLogic.UsernameOrEmailAlreadyTakenAsync(newUser.UserName, newUser.UserEmail))
            return TypedResults.Conflict("User or email name is already taken.");
        if (string.IsNullOrEmpty(newUser.UserAddress) || string.IsNullOrEmpty(newUser.UserZipCode))
            return TypedResults.BadRequest("User address or zip code. cannot be empty.");
        if (newUser.UserZipCode?.Length != 4)
            return TypedResults.BadRequest(string.Format("User zip code must be 4 digits long. {0} is {1} digits long.", newUser.UserZipCode, newUser.UserZipCode?.Length));

        newUser.UserId = 0;
        newUser.UserCity = await userLogic.GetCityFromZipCodeAsync(newUser.UserZipCode);
        newUser.UserZipCode = string.IsNullOrEmpty(newUser.UserCity) ? null : newUser.UserZipCode;
        newUser.RegisteredOn = DateTime.UtcNow;

        //Insert user and create a logic for the JWT
        try
        {
            // Inser user into database
            var user = await userLogic.RegisterUserAsync(newUser);
            if (user is null) return TypedResults.NotFound("Something happened with the database that prevented the insertion of the user.");
            // Generate a JWT token.
            var jwt = userLogic.GetJwtToken(user);
            logger.Information("user-service: Exiting method Register");

            return string.IsNullOrEmpty(jwt) ? TypedResults.BadRequest("User has been successfully registered but failed to retrieve the JWT token.") : TypedResults.Created("/", jwt);
        }
        catch (Exception ex)
        {
            return TypedResults.BadRequest(ex.Message);
        }
    }

    /// <summary>
    /// Gets the user by id.
    /// </summary>
    /// <param name="id">Id of the user.</param>
    /// <param name="userLogic"><see cref="IUserLogic"/> instance.</param>
    /// <returns>User data for the user.</returns>
    public static async Task<Results<Ok<UserWithAdsDataDTO>, Ok<UserDataWithoutAdsDTO>, BadRequest<string>>> GetUserById(int id, IUserLogic userLogic, Serilog.ILogger logger, HttpContext httpContext)
    {
        var requestId = httpContext?.TraceIdentifier ?? "Unknown";
        logger.Information("user-service: GetUserById method called. RequestID: {@requestId}", requestId);

        var user = await userLogic.GetUserByIdAsync(id);

        if (user is null)
            return TypedResults.BadRequest("User with the specified doesn't exist.");

        var ads = await userLogic.GetUsersAddsAsync(id);
        if (!ads.Any())
        {
            var userWithoutAds = new UserDataWithoutAdsDTO(user);
            return TypedResults.Ok(userWithoutAds);
        }

        var userData = new UserWithAdsDataDTO(user, ads);


        //Get all the ads from that user.
        logger.Information("user-service: Exiting method GetUserById");

        return TypedResults.Ok(userData);
    }

    /// <summary>
    /// Deletes the user by id.
    /// </summary>
    /// <param name="id">User id.</param>
    /// <param name="userLogic"><see cref="IUserLogic"/> instance.</param>
    /// <returns>Result of the Delete user by id request.</returns>
    public static async Task<Results<NoContent, BadRequest<string>>> DeleteUserById(int id, IUserLogic userLogic, Serilog.ILogger logger, HttpContext httpContext)
    {
        var requestId = httpContext?.TraceIdentifier ?? "Unknown";
        logger.Information("user-service: DeleteUserById method called. RequestID: {@requestId}", requestId);

        var user = await userLogic.GetUserByIdAsync(id);

        if (user is null)
            return TypedResults.BadRequest("User with the specified doesn't exist.");

        await userLogic.DeleteUserAsync(user);

        logger.Information("user-service: Exiting method DeleteUserById");
        return TypedResults.NoContent();
    }

    /// <summary>
    /// Updates the user name by id.
    /// </summary>
    /// <param name="id">Id of the user.</param>
    /// <param name="userName">The new name of the user.</param>
    /// <param name="userLogic"><see cref="IUserLogic"/> instance.</param>
    /// <returns></returns>
    public static async Task<Results<Ok<UserDataDTO>, BadRequest<string>, Conflict<string>>> UpdateUserNameById(int id, string userName, IUserLogic userLogic, Serilog.ILogger logger, HttpContext httpContext)
    {
        var requestId = httpContext?.TraceIdentifier ?? "Unknown";
        logger.Information("user-service: UpdateUserNameById method called. RequestID: {@requestId}", requestId);

        var user = await userLogic.GetUserByIdAsync(id);

        if (user is null)
            return TypedResults.BadRequest("User with the specified doesn't exist.");

        if (string.IsNullOrEmpty(userName))
            return TypedResults.BadRequest("User name cannot be empty.");

        if (await userLogic.UsernameOrEmailAlreadyTakenAsync(userName, user.UserEmail))
            return TypedResults.Conflict("User name is already taken.");

        user.UserName = userName;
        var userData = new UserDataDTO(user);
        if (await userLogic.UpdateUserAsync(user))
            return TypedResults.Ok(userData);


        logger.Information("user-service: Exiting method UpdateUserNameById");
        return TypedResults.BadRequest("Failed to update the user name.");
    }

    /// <summary>
    /// Updates the user data and returns a new JWT token.
    /// </summary>
    /// <param name="newUserData">New user data.</param>
    /// <param name="userLogic"><see cref="UserLogic"/> DI.</param>
    /// <returns><see cref="Results"/>.</returns>
    public static async Task<Results<Ok<string>, BadRequest<string>, Conflict<string>>> UpdateUser(User newUserData, IUserLogic userLogic, Serilog.ILogger logger, HttpContext httpContext)
    {
        var requestId = httpContext?.TraceIdentifier ?? "Unknown";
        logger.Information("user-service: UpdateUser method called. RequestID: {@requestId}", requestId);

        var existingUser = userLogic.GetUserByIdAsync(newUserData.UserId);
        if (existingUser is null)
            return TypedResults.BadRequest("User with the specified id doesn't exist.");
        if (string.IsNullOrEmpty(newUserData.UserName))
            return TypedResults.BadRequest("User name or email cannot be empty.");
        if (!(await userLogic.IsUserNameUniqueAsync(newUserData.UserName)) || !(await userLogic.IsEmailUniqueAsync(newUserData.UserEmail)))
            return TypedResults.Conflict("User or email name is already taken.");
        if (string.IsNullOrEmpty(newUserData.UserAddress) || string.IsNullOrEmpty(newUserData.UserZipCode))
            return TypedResults.BadRequest("User address or zip code. cannot be empty.");
        if (newUserData.UserZipCode?.Length != 4)
            return TypedResults.BadRequest(string.Format("User zip code must be 4 digits long. {0} is {1} digits long.", newUserData.UserZipCode, newUserData.UserZipCode?.Length));
        newUserData.UserCity = await userLogic.GetCityFromZipCodeAsync(newUserData.UserZipCode);
        var isUserUpdated = await userLogic.UpdateUserDataAsync(newUserData);
        if (isUserUpdated)
        {
            var user = await userLogic.GetUserByUsernameOrEmailAndPasswordAsync(newUserData.UserEmail, newUserData.UserPassword);
            var jwt = userLogic.GetJwtToken(user);
            return string.IsNullOrEmpty(jwt) ? TypedResults.BadRequest("User has been successfully registered but failed to retrieve the JWT token.") : TypedResults.Ok(jwt);
        }

        logger.Information("user-service: Exiting method UpdateUser");
        return TypedResults.BadRequest("Failed to update the user.");
    }

    public static async Task<Results<Ok<List<User>>, BadRequest<string>>> GetAllUsers(IUserLogic userLogic, Serilog.ILogger logger, HttpContext httpContext)
    {
        var requestId = httpContext?.TraceIdentifier ?? "Unknown";
        logger.Information("user-service: GetAllUsers method called. RequestID: {@requestId}", requestId);

        var users = await userLogic.GetAllUsersAsync();
        if (users is null)
        {
            logger.Error("user-service: Couldn't find any users.");
            return TypedResults.BadRequest("Couldn't find any users.");
        }

        logger.Information("user-service: Exiting method GetAllUsers");
        return TypedResults.Ok(users);
    }
}
