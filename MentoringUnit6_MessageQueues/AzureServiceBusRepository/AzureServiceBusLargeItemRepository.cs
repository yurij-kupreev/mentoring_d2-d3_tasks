using Microsoft.ServiceBus.Messaging;
using System;
using System.IO;
using System.Threading.Tasks;

namespace AzureServiceBusRepository
{
  public enum AzureServiceBusReceiveMode
  {
    PeekLock = 0,
    ReceiveAndDelete = 1
  }

  public class AzureServiceBusLargeItemRepository<T> : IAzureServiceBusRepository<T>
  {
    public const int MaxMessageHeaderSizeKBytes = 64;

    public readonly QueueClient _queueClient;

    public readonly int _messageQueueMessageMaxSizeKBytes;

    public AzureServiceBusLargeItemRepository(string queueName, int messageQueueMessageMaxSizeKBytes = 256)
      : this(queueName, AzureServiceBusReceiveMode.PeekLock, messageQueueMessageMaxSizeKBytes)
    {
    }

    public AzureServiceBusLargeItemRepository(string connectionString, string queueName, int messageQueueMessageMaxSizeKBytes = 256)
      : this(connectionString, queueName, AzureServiceBusReceiveMode.PeekLock, messageQueueMessageMaxSizeKBytes)
    {
    }

    public AzureServiceBusLargeItemRepository(string queueName, AzureServiceBusReceiveMode receiveMode, int messageQueueMessageMaxSizeKBytes = 256)
    {
      _queueClient = QueueClient.Create(queueName, (ReceiveMode)receiveMode);
      _messageQueueMessageMaxSizeKBytes = messageQueueMessageMaxSizeKBytes;
    }

    public AzureServiceBusLargeItemRepository(string connectionString, string queueName, AzureServiceBusReceiveMode receiveMode, int messageQueueMessageMaxSizeKBytes = 256)
    {
      _queueClient = QueueClient.CreateFromConnectionString(connectionString, queueName, (ReceiveMode)receiveMode);
      _messageQueueMessageMaxSizeKBytes = messageQueueMessageMaxSizeKBytes;
    }

    public void Dispose()
    {
      if (_queueClient != null)
      {
        _queueClient.Close();
      }
    }

    public async Task<T> ReceiveItemAsync()
    {
      var message = await this.ReceiveAsync();
      var data = message.GetBody<T>();

      return data;
    }

    private async Task<BrokeredMessage> ReceiveAsync()
    {
      // Accept a message session from the queue.
      var session = await _queueClient.AcceptMessageSessionAsync(TimeSpan.FromSeconds(30));

      if (session == null)
      {
        throw new TimeoutException();
      }

      //Console.WriteLine("Message session Id: " + session.SessionId);
      //Console.Write("Receiving sub messages");

      var message = await this.ProceedLargeMessageSession(session);

      return message;
    }

    public async Task<BrokeredMessage> ProceedLargeMessageSession(MessageSession session)
    {
      // Create a memory stream to store the large message body.
      var largeMessageStream = new MemoryStream();

      while (true)
      {
        // Receive a sub message
        var subMessage = await session.ReceiveAsync(TimeSpan.FromSeconds(5));

        if (subMessage != null)
        {
          // Copy the sub message body to the large message stream.
          Stream subMessageStream = subMessage.GetBody<Stream>();
          subMessageStream.CopyTo(largeMessageStream);

          // Mark the message as complete.
          subMessage.Complete();
          //Console.Write(".");
        }
        else
        {
          // The last message in the sequence is our completeness criteria.
          //Console.WriteLine("Done!");
          break;
        }
      }

      // Create an aggregated message from the large message stream.
      var largeMessage = new BrokeredMessage(largeMessageStream, true);
      return largeMessage;
    }

    public async Task SendItemAsync(T item)
    {
      var mess = new BrokeredMessage(item);

      await this.SendLargeItemAsync(new BrokeredMessage(item));
    }

    private async Task SendLargeItemAsync(BrokeredMessage message)
    {
      var subMessageBodySize = (_messageQueueMessageMaxSizeKBytes - MaxMessageHeaderSizeKBytes) * 1024; // max size of chunk (bytes)

      // Calculate the number of sub messages required.
      long messageBodySize = message.Size;
      int nrSubMessages = (int)(messageBodySize / subMessageBodySize);
      if (messageBodySize % subMessageBodySize != 0)
      {
        nrSubMessages++;
      }

      // Create a unique session Id.
      string sessionId = Guid.NewGuid().ToString();
      //Console.WriteLine("Message session Id: " + sessionId);
      //Console.Write("Sending {0} sub-messages", nrSubMessages);

      var bodyStream = message.GetBody<Stream>();
      for (int streamOffest = 0; streamOffest < messageBodySize;

          streamOffest += subMessageBodySize)
      {
        // Get the stream chunk from the large message
        var arraySize = (messageBodySize - streamOffest) > subMessageBodySize
            ? subMessageBodySize : messageBodySize - streamOffest;
        var subMessageBytes = new byte[arraySize];
        var result = bodyStream.Read(subMessageBytes, 0, (int)arraySize);
        var subMessageStream = new MemoryStream(subMessageBytes);

        // Create a new message
        var subMessage = new BrokeredMessage(subMessageStream, true);
        subMessage.SessionId = sessionId;

        // Send the message
        await _queueClient.SendAsync(subMessage);
        //Console.Write(".");
      }
      //Console.WriteLine("Done!");
    }
  }
}
