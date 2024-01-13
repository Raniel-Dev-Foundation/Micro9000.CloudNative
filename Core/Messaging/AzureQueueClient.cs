
using Azure.Storage.Queues;
using Microsoft.Extensions.Logging;

namespace Core.Messaging;
public class AzureQueueClient : IAzureQueueClient
{
    private readonly QueueServiceClient _queueServiceClient;
    private readonly ILogger<AzureQueueClient> _logger;

    public AzureQueueClient(QueueServiceClient queueServiceClient, ILogger<AzureQueueClient> logger)
    {
        _queueServiceClient = queueServiceClient;
        _logger = logger;
    }

    public async Task AddMessage(string id, string message, string queueName)
    {
        _logger.LogTrace("Adding message for [Id={id}] to [Queue={QueueName}]", id, queueName);

        var queueClient = GetQueueClient(queueName);
        await queueClient.SendMessageAsync(message);

        _logger.LogInformation("Message [Id={id}] successfully added to [Queue={QueueName}].", id, queueName);
    }

    public async Task EnsureQueueExists(string queueName)
    {
        _logger.LogTrace("Ensuring existence of queue [Name={QueueName}]", queueName);
        var queueClient = GetQueueClient(queueName);
        var queueExists = await queueClient.ExistsAsync();

        if (!queueExists)
        {
            await queueClient.CreateIfNotExistsAsync();
        }
    }

    private QueueClient GetQueueClient(string queueName)
    {
        var client = _queueServiceClient.GetQueueClient(queueName);
        return client;
    }
}
