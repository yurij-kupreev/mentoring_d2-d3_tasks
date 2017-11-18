using System;
using System.Collections.Generic;
using System.Configuration;
using System.Threading;
using System.Windows.Threading;
using DocumentCaptureService.Messaging;
using DocumentCaptureService.RepeatableProcessors;
using DocumentCaptureService.Repositories;
using DocumentCaptureService.ServiceWorkers;
using NLog;

namespace CentralProcessor.Services
{
  public class FileCentralProcessorService
  {
    private readonly List<Thread> _workingThreads;
    private readonly List<DispatcherTimer> _timers;
    private readonly ManualResetEvent _workStopped;

    private const string FilesInputDirectoryKey = "FilesInputDirectory";
    private const string ImagesInputDirectoryKey = "ImagesInputDirectory";
    private const string FilesOutputDirectoryKey = "FilesOutputDirectory";

    private const string BlobContainerNameKey = "BlobContainerName";
    private const string BlobFolderNameKey = "BlobFolderName";
    private const string AzureStorageConnectionStringKey = "AzureStorageConnectionString";

    protected static readonly ILogger Logger = LogManager.GetCurrentClassLogger();

    public FileCentralProcessorService()
    {
      _workStopped = new ManualResetEvent(false);

      _workingThreads = new List<Thread>();
      _timers = new List<DispatcherTimer>();

      InitReceiveProcessor();
    }

    public void Start()
    {
      _workingThreads.ForEach(thr => thr.Start());
      _timers.ForEach(t => t.Start());
    }

    public void Stop()
    {
      _timers.ForEach(t => t.Stop());
      _workStopped.Set();

      _workingThreads.ForEach(thr => thr.Join());
    }

    //private void InitFileProcessor()
    //{
    //  var inputDirectory = ConfigurationManager.AppSettings[FilesInputDirectoryKey];
    //  var outputDirectory = ConfigurationManager.AppSettings[FilesOutputDirectoryKey];

    //  var sourceRepository = new LocalStorageRepository(inputDirectory);
    //  //var destinationRepository = new LocalStorageRepository(outputDirectory);

    //  //var blobContainerName = ConfigurationManager.AppSettings[BlobContainerNameKey];
    //  //var blobFolderName = ConfigurationManager.AppSettings[BlobFolderNameKey];
    //  //var azureStorageConnectionString = ConfigurationManager.AppSettings[AzureStorageConnectionStringKey];
    //  //var blobStorageRepository = new BlobStorageRepository(blobContainerName, azureStorageConnectionString, blobFolderName);

    //  var messenger = new MsmqMessenger(ConfigurationManager.AppSettings["MsmqQueueName"]);
    //  var destinationRepository = new MessengerRepository(messenger);

    //  var newFileAdded = new AutoResetEvent(false);
    //  var watcher = new FileSystemWatcher(inputDirectory);

    //  watcher.Created += (sender, e) => {
    //    Logger.Info($"Create event has been raised. Name: {e.Name}, Path: {e.FullPath}, Event type: {e.ChangeType}");
    //    newFileAdded.Set();
    //  };

    //  var singleFileMoveRepeatableProcessor = new SingleFileMoveRepeatableProcessor(_workStopped, sourceRepository, destinationRepository);
    //  var fileServiceProcessor = new RepeatableWorker(singleFileMoveRepeatableProcessor, _workStopped, newFileAdded);
    //  _workingThreads.Add(fileServiceProcessor.GetThread());
    //  _watchers.Add(watcher);
    //}

    //private void InitImageProcessor()
    //{
    //  var imagesDirectory = ConfigurationManager.AppSettings[ImagesInputDirectoryKey];
    //  var inputDirectory = ConfigurationManager.AppSettings[FilesInputDirectoryKey];

    //  var sourceRepository = new LocalStorageRepository(imagesDirectory);
    //  var destinationRepository = new LocalStorageRepository(inputDirectory);

    //  var newFileAdded = new AutoResetEvent(false);
    //  var watcher = new FileSystemWatcher(imagesDirectory);

    //  watcher.Created += (sender, e) => {
    //    Logger.Info($"Create event has been raised. Name: {e.Name}, Path: {e.FullPath}, Event type: {e.ChangeType}");
    //    newFileAdded.Set();
    //  };

    //  var imagesConversionAndMoveRepeatableProcessor = new ImagesConversionAndMoveRepeatableProcessor(_workStopped, sourceRepository, destinationRepository);
    //  var imageServiceProcessor = new RepeatableWorker(imagesConversionAndMoveRepeatableProcessor, _workStopped, newFileAdded);
    //  _workingThreads.Add(imageServiceProcessor.GetThread());
    //  _watchers.Add(watcher);
    //}

    private void InitReceiveProcessor()
    {
      var outputDirectory = ConfigurationManager.AppSettings[FilesOutputDirectoryKey];

      var messenger = new MsmqMessenger(ConfigurationManager.AppSettings["MsmqQueueName"]);

      var destinationRepository = new LocalStorageRepository(outputDirectory);

      var timerTicked = new AutoResetEvent(false);
      var timer = new DispatcherTimer {Interval = TimeSpan.FromSeconds(5)};
      timer.Tick += (sender, e) => {
        Logger.Info("Timer tick");
        timerTicked.Set();
      };

      var imagesConversionAndMoveRepeatableProcessor = new FileReceiveAndMoveRepeatableProcessor(_workStopped, messenger, destinationRepository);
      var imageServiceProcessor = new RepeatableWorker(imagesConversionAndMoveRepeatableProcessor, _workStopped, timerTicked);
      _workingThreads.Add(imageServiceProcessor.GetThread());
      _timers.Add(timer);
    }
  }
}