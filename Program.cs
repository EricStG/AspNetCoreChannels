using AspNetCoreChannels;
using System.Threading.Channels;

var builder = WebApplication.CreateBuilder(args);

// Setup the channel

builder.Services.AddSingleton(Channel.CreateUnbounded<Model>());
builder.Services.AddSingleton(p => p.GetRequiredService<Channel<Model>>().Reader);
builder.Services.AddSingleton(p => p.GetRequiredService<Channel<Model>>().Writer);

// Setup the hosted service
builder.Services.AddHostedService<ModelService>();

var app = builder.Build();

// Simple HTTP POST that receives an array of models

app.MapPost("/messages", async (ChannelWriter<Model> writer, ICollection<Model> models, CancellationToken cancellationToken) =>
{
    foreach (var model in models)
    {
        await writer.WriteAsync(model, cancellationToken);
    }
    return StatusCodes.Status202Accepted;
});

app.Run();
