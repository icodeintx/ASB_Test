using Azure.Messaging.ServiceBus.Administration;

namespace ASB_Test.Infrastructure;

public class Queue
{
    public async Task CreateQueueIfNotExists(string connectionString, string queueName)
    {
        var adminClient = new ServiceBusAdministrationClient(connectionString);

        if (!await adminClient.QueueExistsAsync(queueName))
        {
            await adminClient.CreateQueueAsync(queueName);
        }
    }
}