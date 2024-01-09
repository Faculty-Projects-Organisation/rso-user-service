using LazyCache;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using RestSharp;
using RSO.Core.BL.LogicModels;
using RSO.Core.Configurations;
using RSO.Core.UserModels;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
using System.Text;
using UserServiceRSO.Repository;

namespace RSO.Core.BL;

/// <summary>
/// Implementation of <see cref="IUserLogic"/> interface.
/// </summary>
public class UserLogic : IUserLogic
{
    private readonly IAppCache _appcache;
    private readonly IUnitOfWork _unitOfWork;
    private readonly JwtSecurityTokenConfiguration _jwtConfiguration;

    /// <summary>
    /// Initializes the <see cref="UserLogic"/> class.
    /// </summary>
    /// <param name="appcache"><see cref="IAppCache"/> instance.</param>
    /// <param name="unitOfWork"><see cref="IUnitOfWork"/> instance.</param>
    /// <param name="jwtConfiguration"><see cref="JwtSecurityTokenConfiguration"/> dependency injection.</param>
    public UserLogic(IAppCache appcache, IUnitOfWork unitOfWork, JwtSecurityTokenConfiguration jwtConfiguration)
    {
        _appcache = appcache;
        _unitOfWork = unitOfWork;
        _jwtConfiguration = jwtConfiguration;
    }

    ///<inheritdoc/>
    public async Task DeleteUserAsync(User user)
    {
        await _unitOfWork.UserRepository.DeleteUserAsync(user);
    }

    ///<inheritdoc/>
    public async Task<string> GetCityFromZipCodeAsync(string userZipCode)
    {
        return await _appcache.GetOrAddAsync($"city_from_zip_code_{userZipCode}", async () =>
         {
             var restRequest = new RestRequest();
             var restClient = new RestClient($"https://api.lavbic.net/kraji/{userZipCode}");
             var response = restClient.ExecuteAsync(restRequest);
             response.Wait();
             var restResponse = await response;

             return restResponse.StatusCode.Equals(HttpStatusCode.OK) ? JsonConvert.DeserializeObject<CityData>(restResponse.Content).City : null;
         });
    }

    ///<inheritdoc/>
    public string GetJwtToken(User existingUser)
    {
        var claims = new Claim[] {
            new(JwtRegisteredClaimNames.Sub,existingUser.UserId.ToString()),
            new(JwtRegisteredClaimNames.Email,existingUser.UserEmail),
            new(JwtRegisteredClaimNames.AuthTime, DateTime.UtcNow.ToString()),
        };

        var signingCredentials = new SigningCredentials(
            new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtConfiguration.SecretKey)),
            SecurityAlgorithms.HmacSha256);

        var jwtToken = new JwtSecurityToken(
            _jwtConfiguration.Issuer,
            _jwtConfiguration.Audience,
            claims,
            DateTime.UtcNow.AddSeconds(-5),
            DateTime.UtcNow.AddDays(7),
            signingCredentials);

        return new JwtSecurityTokenHandler().WriteToken(jwtToken);
    }

    ///<inheritdoc/>
    public async Task<User> GetUserByIdAsync(int id)
    {
        try
        {
            return await _unitOfWork.UserRepository.GetByIdAsync(id);
        }
        catch (Exception ex)
        {
            // Log
            return null;
        }
    }

    ///<inheritdoc/>
    public async Task<User> GetUserByUsernameOrEmailAndPasswordAsync(string emailOrUsername, string password)
    {
        try
        {
            if (await _unitOfWork.UserRepository.UserExistsByUserNameOrEmailOrPasswordAsync(emailOrUsername, password))
                return await _unitOfWork.UserRepository.GetByUsernameOrEmailAndPasswordAsync(emailOrUsername, password);
            return null;
        }
        catch (Exception ex)
        {
            // Log with serilog/seq when needed.

            return null;
        }
    }

    ///<inheritdoc/>
    public async Task<User> RegisterUserAsync(User newUser)
    {
        try
        {
            var user = await _unitOfWork.UserRepository.InsertAsync(newUser);
            await _unitOfWork.SaveChangesAsync();
            return user;
        }
        catch (Exception ex)
        {
            // Log with serilog or seq? when needed.

            return null;
        }
    }

    ///<inheritdoc/>
    public async Task<bool> UsernameOrEmailAlreadyTakenAsync(string userName, string email)
    {
        try
        {
            return await _unitOfWork.UserRepository.UsernameOrEmailAlreadyTakenAsync(userName, email);
        }
        catch (Exception ex)
        {
            return false;
        }
    }

    ///<inheritdoc/>
    public async Task<bool> UpdateUserAsync(User user)
    {
        try
        {
            await _unitOfWork.UserRepository.UpdateUsersNameAsync(user);
        }
        catch (Exception ex)
        {
            return false;
        }

        return true;
    }
}
