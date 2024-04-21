using Azure.Messaging.ServiceBus;

namespace ASB_Test.Services;

public class QueueOneService
{
    private ServiceBusClient _client;

    private ServiceBusReceiver _receiver;

    private ServiceBusSender _sender;

    private string ConnectionString;

    private string QueueName;

    public QueueOneService(string ConnectionString, string QueueName)
    {
        this.ConnectionString = ConnectionString;
        this.QueueName = QueueName;

        if (!StartClient())
        {
            throw new Exception("Failed to start client");
        }
    }

    public async Task<string> ReceiveMessage()
    {
        ServiceBusReceivedMessage msg = await this._receiver.ReceiveMessageAsync(new TimeSpan(0, 0, 3));
        if (msg != null)
        {
            await _receiver.CompleteMessageAsync(msg);

            return msg.Body.ToString();
        }
        else
        {
            return null;
        }
    }

    public async Task<bool> SendMessage(string message)
    {
        try
        {
            await this._sender.SendMessageAsync(new ServiceBusMessage(message));
            return true;
        }
        catch (Exception)
        {
            throw;
        }
    }

    private bool StartClient()
    {
        if (this.ConnectionString != null && this.QueueName != null)
        {
            this._client = new ServiceBusClient(this.ConnectionString);
            this._sender = this._client.CreateSender(this.QueueName);
            this._receiver = this._client.CreateReceiver(this.QueueName);
            return true;
        }
        else
        {
            return false;
        }
    }
}