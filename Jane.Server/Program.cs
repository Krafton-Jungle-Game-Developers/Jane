using MagicOnion;
using MagicOnion.Serialization;
using MagicOnion.Serialization.MemoryPack;
using MagicOnion.Server;
using Microsoft.AspNetCore.Server.Kestrel.Core;

var builder = WebApplication.CreateBuilder(args);
builder.WebHost.UseKestrel(options =>
{
    options.ConfigureEndpointDefaults(endpointOptions =>
    {
        endpointOptions.Protocols = HttpProtocols.Http1AndHttp2;
    });
}).UseUrls("http://jane.jungle-gamedev.com:5000;https://jane.jungle-gamedev.com:5001");

builder.Services.AddGrpc();

MagicOnionSerializerProvider.Default = MemoryPackMagicOnionSerializerProvider.Instance;
builder.Services.AddMagicOnion(options =>
{
    options.MessageSerializer = MemoryPackMagicOnionSerializerProvider.Instance;
});

var app = builder.Build();

app.MapMagicOnionService();
app.MapGet("/", () => "Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");

app.Run();
