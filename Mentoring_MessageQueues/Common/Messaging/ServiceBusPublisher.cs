using System;
using System.ServiceModel;
using Common.Models;
using Microsoft.ServiceBus.Messaging;

namespace Common.Messaging
{
  public class ServiceBusPublisher
  {
    //private const string StatusTopicName = "StatusTopic";
    //private const string SubscriptionName = "StatusSubscription";

    private readonly TopicClient _topicClient;

    public ServiceBusPublisher(string connectionString, string topicName)
    {
      _topicClient = TopicClient.CreateFromConnectionString(connectionString, topicName);
    }

    public void Publish(CustomMessage message)
    {
      message.Body.Position = 0;

      var brokeredMessage = new BrokeredMessage(message.Body, true)
      {
        Label = message.Label
      };

      _topicClient.Send(brokeredMessage);
    }
  }
}