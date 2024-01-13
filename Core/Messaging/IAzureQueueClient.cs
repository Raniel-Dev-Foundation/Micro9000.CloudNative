namespace Core.Messaging;
public interface IAzureQueueClient
{
    Task AddMessage(string id, string message, string queueName);
    Task EnsureQueueExists(string queueName);
}
