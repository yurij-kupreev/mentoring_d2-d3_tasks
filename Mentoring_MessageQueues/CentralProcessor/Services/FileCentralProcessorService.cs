using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading;
using Common.Helpers;
using Common.Messaging;
using Common.Models;
using Common.RepeatableProcessors;
using Common.RepeatableProcessors.FileProcessors;
using Common.RepeatableProcessors.ImageSetProcessors;
using Common.Repositories;
using Common.ServiceWorkers;
using NLog;
using Timer = System.Timers.Timer;

namespace CentralProcessor.Services
{
  public class FileCentralProcessorService
  {
    private readonly List<Thread> _workingThreads;
    private readonly List<Timer> _timers;
    private readonly List<FileSystemWatcher> _watchers;
    private readonly ManualResetEvent _workStopped;
    private readonly List<IRepeatableProcessor> _processors;

    protected static readonly ILogger Logger = LogManager.GetCurrentClassLogger();

    public FileCentralProcessorService()
    {
      _workStopped = new ManualResetEvent(false);

      _workingThreads = new List<Thread>();
      _timers = new List<Timer>();
      _watchers = new List<FileSystemWatcher>();
      _processors = new List<IRepeatableProcessor>();

      InitSingleFileReceive();
      InitImageSetReceive();
      InitStatusSending();
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

    private void InitSingleFileReceive()
    {
      var outputDirectory = ConfigurationManager.AppSettings[AppKeys.FilesOutputDirectoryKey];

      var messenger = new MsmqMessenger(ConfigurationManager.AppSettings[AppKeys.MsmqSingleFileQueueNameKey]);

      var destinationRepository = new LocalStorageRepository(outputDirectory);

      var timerTicked = new AutoResetEvent(false);
      var timer = new Timer {Interval = 1000};
      timer.Elapsed += (sender, e) => {
        Logger.Info("Timer tick");
        timerTicked.Set();
      };

      var imagesConversionAndMoveRepeatableProcessor = new FileReceiveAndMoveRepeatableProcessor(_workStopped, messenger, destinationRepository);
      var imageServiceProcessor = new RepeatableWorker(imagesConversionAndMoveRepeatableProcessor, _workStopped, timerTicked);
      _workingThreads.Add(imageServiceProcessor.GetThread());
      _timers.Add(timer);
      _processors.Add(imagesConversionAndMoveRepeatableProcessor);
    }

    private void InitImageSetReceive()
    {
      var outputDirectory = ConfigurationManager.AppSettings[AppKeys.ImagesInputDirectoryKey];

      var messenger = new MsmqMessenger(ConfigurationManager.AppSettings[AppKeys.MsmqImageSetQueueNameKey]);

      var destinationRepository = new LocalStorageRepository(outputDirectory);

      var timerTicked = new AutoResetEvent(false);
      var timer = new Timer { Interval = 1000 };
      timer.Elapsed += (sender, e) => {
        Logger.Info("Timer tick");
        timerTicked.Set();
      };

      var imagesConversionAndMoveRepeatableProcessor = new FileReceiveAndMoveRepeatableProcessor(_workStopped, messenger, destinationRepository);
      var imageServiceProcessor = new RepeatableWorker(imagesConversionAndMoveRepeatableProcessor, _workStopped, timerTicked);
      _workingThreads.Add(imageServiceProcessor.GetThread());
      _timers.Add(timer);
      _processors.Add(imagesConversionAndMoveRepeatableProcessor);

      InitImageProcessor();
    }

    private void InitImageProcessor()
    {
      var imagesDirectory = ConfigurationManager.AppSettings[AppKeys.ImagesInputDirectoryKey];
      var outputDirectory = ConfigurationManager.AppSettings[AppKeys.FilesOutputDirectoryKey];

      var sourceRepository = new LocalStorageRepository(imagesDirectory);
      var destinationRepository = new LocalStorageRepository(outputDirectory);

      var newFileAdded = new AutoResetEvent(false);
      var watcher = new FileSystemWatcher(imagesDirectory);

      watcher.Created += (sender, e) => {
        Logger.Info($"Create event has been raised. Name: {e.Name}, Path: {e.FullPath}, Event type: {e.ChangeType}");
        newFileAdded.Set();
      };

      var imagesConversionAndMoveRepeatableProcessor = new ImagesConversionAndMoveRepeatableProcessor(_workStopped, sourceRepository, destinationRepository);
      var imageServiceProcessor = new RepeatableWorker(imagesConversionAndMoveRepeatableProcessor, _workStopped, newFileAdded);
      _workingThreads.Add(imageServiceProcessor.GetThread());
      _watchers.Add(watcher);
      _processors.Add(imagesConversionAndMoveRepeatableProcessor);
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

      subscriber.MessageReceived += (sender, message) => {
        var formatter = new BinaryFormatter();

        foreach (var repeatableProcessor in _processors) {
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