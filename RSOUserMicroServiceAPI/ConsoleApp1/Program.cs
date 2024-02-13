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
        Console.WriteLine("UserName: " + reply.UserName + " \nEmail: " + reply.Email + " \nUser Address: " + reply.UserAddress + " \nZip code: " + reply.ZipCode + " \nRegistered on:" + reply.RegisteredOn + " \nId:" + reply.Id);
        Console.ReadKey();
        Console.ReadKey();
    }
}
