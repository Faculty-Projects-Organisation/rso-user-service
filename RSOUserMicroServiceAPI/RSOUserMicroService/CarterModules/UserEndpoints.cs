using Carter;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using RSO.Core.BL;
using RSO.Core.BL.LogicModels;
using RSO.Core.UserModels;

namespace UserServiceRSO.CarterModules;

public class UserEndpoints : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapHealthChecks("/api/user/health").WithTags("Health");

        // Login and register options.
        app.MapPost("/login", Login).WithName(nameof(Login)).
            Produces(StatusCodes.Status200OK).WithDisplayName("Lol").
            Produces(StatusCodes.Status400BadRequest).
            AllowAnonymous().WithTags("Users");

        app.MapPost("/register", Register).WithName(nameof(Register)).
            Produces(StatusCodes.Status201Created).
            Produces(StatusCodes.Status400BadRequest).
            Produces(StatusCodes.Status404NotFound).WithDescription("Ne najdem").
            AllowAnonymous().WithTags("Users");

        // Group for /api/user endpoinds.
        var group = app.MapGroup("/api/user/");

        // Methods for  /api/user/id endpoints.
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

        //group.MapGet("/health", HealthCheck).WithName(nameof(HealthCheck)).
        //   Produces(StatusCodes.Status200OK).WithTags("Health");
    }

    /// <summary>
    /// Performs login and gets the JWT token.
    /// </summary>
    /// <param name="loginCredentials">The login credentials.</param>
    /// <param name="userLogic"><see cref="IUserLogic"/> dependency injection.</param>
    /// <returns>A JWT token as a string.</returns>
    public static async Task<Results<Ok<string>, BadRequest<string>>> Login([FromBody] LoginCredentials loginCredentials, IUserLogic userLogic)
    {
        if (string.IsNullOrEmpty(loginCredentials.EmailorUsername) || string.IsNullOrEmpty(loginCredentials.Password))
            return TypedResults.BadRequest("Username (or email) and password cannot be empty.");
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
    public static async Task<Results<Created<string>, NotFound<string>, BadRequest<string>>> Register(User newUser, IUserLogic userLogic)
    {
        //SKIP VALIDATION (IMPLEMENT IT IF TIME REMAINS)
        newUser.UserId = 0;
        newUser.UserCity = await userLogic.GetCityFromZipCodeAsync(newUser.UserZipCode);
        newUser.UserZipCode = string.IsNullOrEmpty(newUser.UserCity) ? null : newUser.UserZipCode;

        //Insert user and create a logic for the JWT
        try
        {
            // Inser user into database
            var user = await userLogic.RegisterUserAsync(newUser);
            if (user is null) return TypedResults.NotFound("Something happened with the database that prevented the insertion of the user.");
            // Generate a JWT token.
            var jwt = userLogic.GetJwtToken(user);
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
    public static async Task<Results<Ok<UserDataDTO>, BadRequest<string>>> GetUserById(int id, IUserLogic userLogic)
    {
        var user = await userLogic.GetUserByIdAsync(id);

        if (user is null)
            return TypedResults.BadRequest("User with the specified doesn't exist.");
        var userData = new UserDataDTO(user);

        return TypedResults.Ok(userData);
    }

    /// <summary>
    /// Deletes the user by id.
    /// </summary>
    /// <param name="id">User id.</param>
    /// <param name="userLogic"><see cref="IUserLogic"/> instance.</param>
    /// <returns>Result of the Delete user by id request.</returns>
    public static async Task<Results<NoContent, BadRequest<string>>> DeleteUserById(int id, IUserLogic userLogic)
    {
        var user = await userLogic.GetUserByIdAsync(id);

        if (user is null)
            return TypedResults.BadRequest("User with the specified doesn't exist.");

        await userLogic.DeleteUserAsync(user);

        return TypedResults.NoContent();
    }

    /// <summary>
    /// Updates the user name by id.
    /// </summary>
    /// <param name="id">Id of the user.</param>
    /// <param name="userName">The new name of the user.</param>
    /// <param name="userLogic"><see cref="IUserLogic"/> instance.</param>
    /// <returns></returns>
    public static async Task<Results<Ok<UserDataDTO>, BadRequest<string>,Conflict<string>>> UpdateUserNameById(int id, string userName, IUserLogic userLogic)
    {
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

        return TypedResults.BadRequest("Failed to update the user name.");
    }

    //[AllowAnonymous]
    //public static async Task<Results<Ok<string>, BadRequest<string>>> HealthCheck()
    //{
    //    try
    //    {
    //           var healthCheckResult = await HealthCheckService.CheckHealthAsync();
    //        return healthCheckResult.CheckStatus == HealthCheckResult.Healthy ? TypedResults.Ok("Healthy") : TypedResults.BadRequest("Unhealthy");
    //    }
    //    catch (Exception ex)
    //    {
    //        return TypedResults.BadRequest(ex.Message);
    //    }
    //}
}
