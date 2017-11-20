using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.IO.MemoryMappedFiles;
using System.Linq.Expressions;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading;
using Common.Helpers;
using Common.Messaging;
using Common.Models;
using NLog;
using Timer = System.Timers.Timer;

namespace RemoteControl.Services
{
  public class RemoteControlService
  {

    private readonly List<Thread> _workingThreads;
    private readonly List<Timer> _timers;
    private readonly List<FileSystemWatcher> _watchers;
    private readonly ManualResetEvent _workStopped;

    protected static readonly ILogger Logger = LogManager.GetCurrentClassLogger();

    public RemoteControlService()
    {
      _workStopped = new ManualResetEvent(false);

      _workingThreads = new List<Thread>();
      _timers = new List<Timer>();
      _watchers = new List<FileSystemWatcher>();

      InitStatusRequest();
      InitStatusReceive();

      
    }

    public void Start()
    {
      _workingThreads.ForEach(thr => thr.Start());
      _timers.ForEach(t => t.Start());
      _watchers.ForEach(w => w.EnableRaisingEvents = true);
    }

    public void Stop()
    {
      _timers.ForEach(t => t.Stop());
      _watchers.ForEach(w => w.EnableRaisingEvents = false);
      _workStopped.Set();

      _workingThreads.ForEach(thr => thr.Join());
    }

    private void InitStatusRequest()
    {
      var pubSub = new ServiceBusPublisher(ConfigurationManager.AppSettings[AppKeys.AzureServiceBusConnectionStringKey], ConfigurationManager.AppSettings[AppKeys.StatusTopicNameKey]);
      pubSub.Publish(new CustomMessage { Label = "HELLO !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!", Body = new MemoryStream(Encoding.Unicode.GetBytes(@"here is STREAM BODY")) });
    }

    private void InitStatusReceive()
    {
      var subscriber = new ServiceBusSubscriber(
        ConfigurationManager.AppSettings[AppKeys.AzureServiceBusConnectionStringKey],
        ConfigurationManager.AppSettings[AppKeys.StatusResponseTopicNameKey],
        ConfigurationManager.AppSettings[AppKeys.StatusResponseSubscriptionNameKey]
        );

      subscriber.MessageReceived += (sender, message) => {
        var formatter = new BinaryFormatter();

        var status = formatter.Deserialize(message.Body) as ProcessorStatus;

        Console.WriteLine(status?.ToString());
      };
    }

  }
}