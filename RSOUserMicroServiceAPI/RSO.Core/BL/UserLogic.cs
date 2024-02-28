using LazyCache;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using RestSharp;
using RSO.Core.BL.LogicModels;
using RSO.Core.Configurations;
using RSO.Core.UserModels;
using RSO.Core.AdModels;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
using System.Text;
using Grpc.Net.Client;
using UserServiceRSO.Repository;
using RSOUserMicroservice;
using System.Globalization;

namespace RSO.Core.BL;

/// <summary>
/// Implementation of <see cref="IUserLogic"/> interface.
/// </summary>
public class UserLogic : IUserLogic
{
  private readonly IAppCache _appcache;
  private readonly IUnitOfWork _unitOfWork;
  private readonly JwtSecurityTokenConfiguration _jwtConfiguration;
  private readonly CrossEndpointsFunctionalityConfiguration _crossEndpointsFunctionalityConfiguration;

  /// <summary>
  /// Initializes the <see cref="UserLogic"/> class.
  /// </summary>
  /// <param name="appcache"><see cref="IAppCache"/> instance.</param>
  /// <param name="unitOfWork"><see cref="IUnitOfWork"/> instance.</param>
  /// <param name="jwtConfiguration"><see cref="JwtSecurityTokenConfiguration"/> dependency injection.</param>
  /// <param name="crossEndpointsFunctionalityConfiguration"><see cref="CrossEndpointsFunctionalityConfiguration"/> DI.</param>
  public UserLogic(IAppCache appcache, IUnitOfWork unitOfWork, JwtSecurityTokenConfiguration jwtConfiguration, CrossEndpointsFunctionalityConfiguration crossEndpointsFunctionalityConfiguration)
  {
    _appcache = appcache;
    _unitOfWork = unitOfWork;
    _jwtConfiguration = jwtConfiguration;
    _crossEndpointsFunctionalityConfiguration = crossEndpointsFunctionalityConfiguration;
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
            new("RegisteredOn",existingUser.RegisteredOn.ToString()),
            new("UserZipCode",existingUser.UserZipCode),
            new("UserCity",existingUser.UserCity),
            new("UserAddress",existingUser.UserAddress),
            new("UserName",existingUser.UserName)
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

  public async Task<List<User>> GetAllUsersAsync()
  {
    try
    {
      return await _unitOfWork.UserRepository.GetAllAsync();
    }
    catch (Exception ex)
    {
      return null;
    }
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

  ///<inheritdoc/>
  public async Task<bool> UpdateUserDataAsync(User userData)
  {
    try
    {
      await _unitOfWork.UserRepository.UpdateUserDataAsync(userData);
    }
    catch (Exception ex)
    {
      return false;
    }
    return true;
  }

  ///<inheritdoc/>
  public async Task<bool> IsEmailUniqueAsync(string userEmail)
  {
    try
    {
      return await _unitOfWork.UserRepository.GetEmailOccurrenceAsync(userEmail) <= 1;
    }
    catch (Exception ex)
    {
      return false;
    }
  }

  ///<inheritdoc/>
  public async Task<bool> IsUserNameUniqueAsync(string userName)
  {
    try
    {
      return await _unitOfWork.UserRepository.GetUserNameOcurrenceAsync(userName) <= 1;
    }
    catch (Exception ex)
    {
      return false;
    }
  }

  ///<inheritdoc/>
  public async Task<List<Ad>> GetUsersAdsAsync(int userId)
  {
    var userURL = _crossEndpointsFunctionalityConfiguration.GetAdsByUserIdEndpoint + userId;
    var restClient = new RestClient(userURL);
    var restRequest = new RestRequest();
    var response = restClient.ExecuteAsync(restRequest);
    response.Wait();
    var restResponse = await response;
    var ads = JsonConvert.DeserializeObject<List<Ad>>(restResponse.Content);
    if (!ads.Any())
      return Enumerable.Empty<Ad>().ToList();
    return ads.Where(ad => ad.UserId == userId).ToList();
  }

  ///<inheritdoc/>
  public List<Ad> GetUsersAdsByRPC(int userId)
  {
    var channel = GrpcChannel.ForAddress("http://ad-cip-service:80/ads/api/grpc/" + userId);
    var client = new AdProto.AdProtoClient(channel);

    var reply = client.GetAdsByUserId(new AdByIdUserIdRequest { UserId = userId });
    if (!reply.Ads.Any())
      return Enumerable.Empty<Ad>().ToList();
    var ads = new List<Ad>();
    foreach (var adItem in reply.Ads)
    {
      var ad = new Ad()
      {
        ID = adItem.Id,
        Category = adItem.Category,
        Thing = adItem.Thing,
        Price = adItem.Price,
        UserId = userId,
        PostTime = DateTime.ParseExact(adItem.PublishDate, "MM/dd/yyyy HH:mm:ss", new CultureInfo("si-SI"))
      };
      ads.Add(ad);
    }
    return ads;
  }
}
