using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Threading;
using DocumentCaptureService.Messaging;
using DocumentCaptureService.RepeatableProcessors;
using DocumentCaptureService.Repositories;
using DocumentCaptureService.ServiceWorkers;
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

    private const string FilesInputDirectoryKey = "FilesInputDirectory";
    private const string ImagesInputDirectoryKey = "ImagesInputDirectory";
    private const string FilesOutputDirectoryKey = "FilesOutputDirectory";

    private const string BlobContainerNameKey = "BlobContainerName";
    private const string BlobFolderNameKey = "BlobFolderName";
    private const string AzureStorageConnectionStringKey = "AzureStorageConnectionString";

    private const string MsmqSingleFileQueueNameKey = "MsmqSingleFileQueueName";
    private const string MsmqImageSetQueueNameKey = "MsmqImageSetQueueName";

    protected static readonly ILogger Logger = LogManager.GetCurrentClassLogger();

    public FileCentralProcessorService()
    {
      _workStopped = new ManualResetEvent(false);

      _workingThreads = new List<Thread>();
      _timers = new List<Timer>();
      _watchers = new List<FileSystemWatcher>();

      InitSingleFileReceive();
      InitImageSetReceive();
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
      var outputDirectory = ConfigurationManager.AppSettings[FilesOutputDirectoryKey];

      var messenger = new MsmqMessenger(ConfigurationManager.AppSettings[MsmqSingleFileQueueNameKey]);

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
    }

    private void InitImageSetReceive()
    {
      var outputDirectory = ConfigurationManager.AppSettings[ImagesInputDirectoryKey];

      var messenger = new MsmqMessenger(ConfigurationManager.AppSettings[MsmqImageSetQueueNameKey]);

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

      InitImageProcessor();
    }

    private void InitImageProcessor()
    {
      var imagesDirectory = ConfigurationManager.AppSettings[ImagesInputDirectoryKey];
      var outputDirectory = ConfigurationManager.AppSettings[FilesOutputDirectoryKey];

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
    }
  }
}