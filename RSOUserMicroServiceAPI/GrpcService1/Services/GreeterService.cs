using Grpc.Core;
using RSO.Core.BL;

namespace GrpcService1.Services;

public class GreeterService : Greeter.GreeterBase
{

    private readonly IUserLogic userLogic;
    public GreeterService(IUserLogic userLogic)
    {
        this.userLogic = userLogic;
    }




    public override Task<HelloReply> SayHello(HelloRequest request, ServerCallContext context)
    {
        return Task.FromResult(new HelloReply
        {
            Message = "Hello " + request.Name
        });
    }

    public override Task<UserByIdReply> ReturnUserById(UserByIdRequest request, ServerCallContext context)
    {
        var user = userLogic.GetUserByIdAsync(request.Id).Result;

        return Task.FromResult(new UserByIdReply
        {
            UserName = user.UserName,
            Email = user.UserEmail,
            UserAddress = user.UserAddress,
            ZipCode = user.UserZipCode,
            RegisteredOn = user.RegisteredOn.ToString(),
            Id = user.UserId
        });
    }
}
