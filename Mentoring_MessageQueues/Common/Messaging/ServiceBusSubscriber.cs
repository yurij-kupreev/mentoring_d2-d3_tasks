using System;
using System.IO;
using Common.Models;
using Microsoft.ServiceBus.Messaging;

namespace Common.Messaging
{
  public class ServiceBusSubscriber
  {
    public event EventHandler<CustomMessage> MessageReceived;

    private readonly SubscriptionClient _subscriptionClient;

    public ServiceBusSubscriber(string connectionString, string topicName, string subscriptionName)
    {
      _subscriptionClient = SubscriptionClient.CreateFromConnectionString(connectionString, topicName, subscriptionName);

      this.CreateHandler();
    }

    private void CreateHandler()
    {
      _subscriptionClient.OnMessage(message =>
      {
        var customMessage = new CustomMessage
        {
          Label = message.Label,
          Body = message.GetBody<Stream>()
        };

        MessageReceived?.Invoke(this, customMessage);
      });
    }
  }
}