using MagicOnion;
using MagicOnion.Serialization;
using MagicOnion.Serialization.MemoryPack;
using MagicOnion.Server;

var builder = WebApplication.CreateBuilder(args);

MagicOnionSerializerProvider.Default = MemoryPackMagicOnionSerializerProvider.Instance;

builder.Services.AddGrpc();
builder.Services.AddMagicOnion();


var app = builder.Build();

app.MapMagicOnionService();

app.Run();
