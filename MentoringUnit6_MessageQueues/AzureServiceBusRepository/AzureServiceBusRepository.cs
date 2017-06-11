using Microsoft.ServiceBus.Messaging;

namespace AzureServiceBusRepository
{
  public enum AzureServiceBusReceiveMode
  {
    PeekLock = 0,
    ReceiveAndDelete = 1
  }

  public class AzureServiceBusRepository<T> : IAzureServiceBusRepository<T>
  {
    public readonly QueueClient _queueClient;

    public AzureServiceBusRepository(string queueName)
    {
      _queueClient = QueueClient.Create(queueName);
    }

    public AzureServiceBusRepository(string queueName, AzureServiceBusReceiveMode receiveMode)
    {
      _queueClient = QueueClient.Create(queueName, (ReceiveMode)receiveMode);
    }

    public AzureServiceBusRepository(string connectionString, string queueName)
    {
      _queueClient = QueueClient.CreateFromConnectionString(connectionString, queueName);
    }

    public AzureServiceBusRepository(string connectionString, string queueName, AzureServiceBusReceiveMode receiveMode)
    {
      _queueClient = QueueClient.CreateFromConnectionString(connectionString, queueName, (ReceiveMode)receiveMode);
    }

    public void Dispose()
    {
      if (_queueClient != null)
      {
        _queueClient.Close();
      }
    }

    public T GetItem()
    {
      var message = _queueClient.Receive();
      var data = message.GetBody<T>();

      return data;
    }

    public void SendItem(T item)
    {
      _queueClient.Send(new BrokeredMessage(item));
    }
  }
}
