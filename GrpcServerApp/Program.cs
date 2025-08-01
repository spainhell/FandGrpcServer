using GrpcServerLib.Services;
using Microsoft.AspNetCore.Server.Kestrel.Core;

var builder = WebApplication.CreateBuilder(args);
// builder.WebHost.ConfigureKestrel(serverOptions =>
// {
//     serverOptions.ListenLocalhost(7877,
//         listenOptions => { listenOptions.Protocols = HttpProtocols.Http1AndHttp2; }
//     );
// });


// Add services to the container.
builder.Services.AddGrpc();

var app = builder.Build();

// Configure the HTTP request pipeline.
app.MapGrpcService<RdbService>();
app.MapGet("/", () => "Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");

app.Run();
