using MagicOnion;
using MagicOnion.Serialization;
using MagicOnion.Serialization.MemoryPack;
using MagicOnion.Server;
using Microsoft.AspNetCore.Server.Kestrel.Core;

MagicOnionSerializerProvider.Default = MemoryPackMagicOnionSerializerProvider.Instance;

var builder = WebApplication.CreateBuilder(args);

builder.WebHost.ConfigureKestrel(options =>
{
    // WORKAROUND: Accept HTTP/2 only to allow insecure HTTP/2 connections during development.
    options.ConfigureEndpointDefaults(endpointOptions =>
    {
        endpointOptions.Protocols = HttpProtocols.Http2;
    });
});

builder.Services.AddGrpc();
builder.Services.AddMagicOnion();

var app = builder.Build();

app.MapMagicOnionService();
app.MapGet("/", () => "Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");

app.Run();
