using HelpViewer;
using GrpcServerLib.Services;
using GrpcServerApp.Components;
using Microsoft.AspNetCore.Server.Kestrel.Core;

var builder = WebApplication.CreateBuilder(args);
builder.WebHost.ConfigureKestrel(serverOptions =>
{
    serverOptions.ListenLocalhost(7877,
        listenOptions => { listenOptions.Protocols = HttpProtocols.Http1AndHttp2; }
    );
});

// Add services to the container.
builder.Services.AddGrpc();
builder.Services.AddRazorComponents().AddInteractiveServerComponents();

// Register the FandHelpViewer service
builder.Services.AddSingleton<HelpFile>(sp => new HelpFile("FANDHLP.000", "FANDHLP.T00"));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseStaticFiles();
app.UseAntiforgery();

// Map gRPC services
app.MapGrpcService<RdbService>();

// Map Blazor components
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
