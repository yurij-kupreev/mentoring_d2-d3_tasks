using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading;
using Common.Builders;
using Common.Helpers;
using Common.Messaging;
using Common.Models;
using Common.RepeatableProcessors;
using Common.RepeatableProcessors.FileProcessors;
using Common.RepeatableProcessors.ImageSetProcessors;
using Common.Repositories;
using Common.ServiceWorkers;
using NLog;

namespace DocumentCaptureService.Services
{
  public class FileProcessService
  {
    private readonly List<Thread> _workingThreads;
    private readonly List<FileSystemWatcher> _watchers;
    private readonly ManualResetEvent _workStopped;
    private readonly List<IRepeatableProcessor> _processors;

    protected static readonly ILogger Logger = LogManager.GetCurrentClassLogger();

    public FileProcessService()
    {
      _workStopped = new ManualResetEvent(false);

      _workingThreads = new List<Thread>();
      _watchers = new List<FileSystemWatcher>();
      _processors = new List<IRepeatableProcessor>();

      InitFileProcessor();
      InitImageProcessor();
      InitStatusSending();
      
    }

    public void Start()
    {
      _workingThreads.ForEach(thr => thr.Start());
      _watchers.ForEach(w => w.EnableRaisingEvents = true);
    }

    public void Stop()
    {
      _watchers.ForEach(w => w.EnableRaisingEvents = false);
      _workStopped.Set();

      _workingThreads.ForEach(thr => thr.Join());
    }

    private void InitFileProcessor()
    {
      var inputDirectory = ConfigurationManager.AppSettings[AppKeys.FilesInputDirectoryKey];
      var sourceRepository = new LocalStorageRepository(inputDirectory);

      var messenger = new MsmqMessenger(ConfigurationManager.AppSettings[AppKeys.MsmqSingleFileQueueNameKey]);

      var workerBuilder = WorkerBuilderFactory.Create()
        .WithFileSystemWatcherNonStoppedWaitHandle(inputDirectory)
        .WithWorkStoppedHandle(_workStopped)
        .WithFileGetAndSendRepeatableProcessor(sourceRepository, messenger);

      var fileServiceProcessor = workerBuilder.Build();

      _workingThreads.Add(fileServiceProcessor.GetThread());
      _watchers.Add(workerBuilder.WorkerProcessorsBuilder.WorkersWaitHandlesBuilder.FileSystemWatcher);
      _processors.Add(workerBuilder.RepeatableProcessor);
    }

    private void InitImageProcessor()
    {
      var imagesDirectory = ConfigurationManager.AppSettings[AppKeys.ImagesInputDirectoryKey];

      var sourceRepository = new LocalStorageRepository(imagesDirectory);
      var messenger = new MsmqMessenger(ConfigurationManager.AppSettings[AppKeys.MsmqImageSetQueueNameKey]);

      var workerBuilder = WorkerBuilderFactory.Create()
        .WithFileSystemWatcherNonStoppedWaitHandle(imagesDirectory)
        .WithWorkStoppedHandle(_workStopped)
        .WithFileGetAndSendRepeatableProcessor(sourceRepository, messenger);

      var fileServiceProcessor = workerBuilder.Build();

      _workingThreads.Add(fileServiceProcessor.GetThread());
      _watchers.Add(workerBuilder.WorkerProcessorsBuilder.WorkersWaitHandlesBuilder.FileSystemWatcher);
      _processors.Add(workerBuilder.RepeatableProcessor);
    }

    private void InitStatusSending()
    {
      var subscriber = new ServiceBusSubscriber(
        ConfigurationManager.AppSettings[AppKeys.AzureServiceBusConnectionStringKey],
        ConfigurationManager.AppSettings[AppKeys.StatusTopicNameKey],
        ConfigurationManager.AppSettings[AppKeys.StatusSubscriptionNameKey]);

      var publisher = new ServiceBusPublisher(
        ConfigurationManager.AppSettings[AppKeys.AzureServiceBusConnectionStringKey],
        ConfigurationManager.AppSettings[AppKeys.StatusResponseTopicNameKey]
        );

      subscriber.MessageReceived += (sender, message) =>
      {
        var formatter = new BinaryFormatter();

        foreach (var repeatableProcessor in _processors)
        {
          var memoryStream = new MemoryStream();
          formatter.Serialize(memoryStream, repeatableProcessor.GetProcessorStatus());

          publisher.Publish(new CustomMessage
          {
            Label = string.Empty,
            Body = memoryStream
          });

          Logger.Info($"{repeatableProcessor.GetType().Name} status sended.");
        }
      };
    }
  }
}
