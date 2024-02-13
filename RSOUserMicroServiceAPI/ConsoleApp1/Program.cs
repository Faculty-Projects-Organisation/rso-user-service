using Grpc.Net.Client;
using GrpcService1;

namespace ConsoleApp1;

public static class Program
{
    static void Main(string[] args)
    {                              
        var channel = GrpcChannel.ForAddress("https://localhost:7021");
        var client = new Greeter.GreeterClient(channel);

        var reply = client.ReturnUserById(new UserByIdRequest { Id = 4});
        Console.WriteLine("User: " + reply.UserName + " " + reply.Email + " " + reply.UserAddress + " " + reply.ZipCode + " " + reply.RegisteredOn + " " + reply.Id);
        Console.ReadKey();
        Console.ReadKey();
    }
}
