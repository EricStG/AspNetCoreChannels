using System.Threading.Channels;

namespace AspNetCoreChannels;

public class ModelService : BackgroundService
{
    private readonly ChannelReader<Model> _channelReader;
    private readonly ILogger<ModelService> _logger;

    public ModelService(ChannelReader<Model> channelReader, ILogger<ModelService> logger)
    {
        _channelReader = channelReader;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        // https://devblogs.microsoft.com/dotnet/an-introduction-to-system-threading-channels/
        while (await _channelReader.WaitToReadAsync(stoppingToken))
        {
            while (_channelReader.TryRead(out var item))
            {
                _logger.LogInformation("Message is `{Message}`", item.Message);
                await Task.Delay(5000, stoppingToken);
            }
        }
    }
}
