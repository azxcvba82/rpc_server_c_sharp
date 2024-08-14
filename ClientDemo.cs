// See https://aka.ms/new-console-template for more information

using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Grpc.Core;
using Grpc.Net.Client;
using GrpcClient;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json.Linq;

// The port number must match the port of the gRPC server.

var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(""));
List<Claim> claimList = new List<Claim>();
var claims = claimList.ToArray<Claim>();

var token = new JwtSecurityToken(
    issuer: "",
    audience: "",
    claims: claims,
    notBefore: DateTime.Now,
     expires: DateTime.Now.AddDays(1),
    signingCredentials: new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256)
);
string jwtToken = new JwtSecurityTokenHandler().WriteToken(token);

var channelOptions = new GrpcChannelOptions
{
    Credentials = ChannelCredentials.SecureSsl,
    HttpClient = new HttpClient
    {
        DefaultRequestHeaders =
        {
            { "Authorization", $"Bearer {jwtToken}" }
        }
    }
};
using var channel = GrpcChannel.ForAddress("https://localhost:7250", channelOptions);
//var client = new Greeter.GreeterClient(channel);
//var reply = await client.SayHelloAsync(
//                  new HelloRequest { Name = "GreeterClient" });
//Console.WriteLine("Greeting: " + reply.Message);
//Console.WriteLine("Press any key to exit...");
//Console.ReadKey();


var client = new billingPeriod.billingPeriodClient(channel);
for (int i = 0; i < 5; i++) { 
var reply = await client.GetDataAsync(
                  new StartDate { Date = $"2024-01-0{i+1}" });
Console.WriteLine("res: " + JObject.Parse(reply.Result)["id"]+" / "+ reply.Result);
}
Console.WriteLine("Press any key to exit...");
Console.ReadKey();