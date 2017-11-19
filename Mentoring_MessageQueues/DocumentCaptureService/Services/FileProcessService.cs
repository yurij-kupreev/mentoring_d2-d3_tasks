using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Threading;
using DocumentCaptureService.Messaging;
using DocumentCaptureService.RepeatableProcessors;
using DocumentCaptureService.RepeatableProcessors.FileProcessors;
using DocumentCaptureService.RepeatableProcessors.ImageSetProcessors;
using DocumentCaptureService.Repositories;
using DocumentCaptureService.ServiceWorkers;
using NLog;

namespace DocumentCaptureService.Services
{
  public class FileProcessService
  {
    private readonly List<Thread> _workingThreads;
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

    public FileProcessService()
    {
      _workStopped = new ManualResetEvent(false);

      _workingThreads = new List<Thread>();
      _watchers = new List<FileSystemWatcher>();

      InitFileProcessor();
      InitImageProcessor();
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
      var inputDirectory = ConfigurationManager.AppSettings[FilesInputDirectoryKey];
      var outputDirectory = ConfigurationManager.AppSettings[FilesOutputDirectoryKey];

      var sourceRepository = new LocalStorageRepository(inputDirectory);
      //var destinationRepository = new LocalStorageRepository(outputDirectory);

      //var blobContainerName = ConfigurationManager.AppSettings[BlobContainerNameKey];
      //var blobFolderName = ConfigurationManager.AppSettings[BlobFolderNameKey];
      //var azureStorageConnectionString = ConfigurationManager.AppSettings[AzureStorageConnectionStringKey];
      //var blobStorageRepository = new BlobStorageRepository(blobContainerName, azureStorageConnectionString, blobFolderName);

      var messenger = new MsmqMessenger(ConfigurationManager.AppSettings[MsmqSingleFileQueueNameKey]);
      //var destinationRepository = new MessengerRepository(messenger);

      var newFileAdded = new AutoResetEvent(false);
      var watcher = new FileSystemWatcher(inputDirectory);

      watcher.Created += (sender, e) =>
      {
        Logger.Info($"Create event has been raised. Name: {e.Name}, Path: {e.FullPath}, Event type: {e.ChangeType}");
        newFileAdded.Set();
      };

      var singleFileMoveRepeatableProcessor = new FileGetAndSendRepeatableProcessor(_workStopped, sourceRepository, messenger);
      var fileServiceProcessor = new RepeatableWorker(singleFileMoveRepeatableProcessor, _workStopped, newFileAdded);
      _workingThreads.Add(fileServiceProcessor.GetThread());
      _watchers.Add(watcher);
    }

    private void InitImageProcessor()
    {
      var imagesDirectory = ConfigurationManager.AppSettings[ImagesInputDirectoryKey];
      //var inputDirectory = ConfigurationManager.AppSettings[FilesInputDirectoryKey];

      var sourceRepository = new LocalStorageRepository(imagesDirectory);
      //var destinationRepository = new LocalStorageRepository(inputDirectory);
      var messenger = new MsmqMessenger(ConfigurationManager.AppSettings[MsmqImageSetQueueNameKey]);

      var newFileAdded = new AutoResetEvent(false);
      var watcher = new FileSystemWatcher(imagesDirectory);

      watcher.Created += (sender, e) => {
        Logger.Info($"Create event has been raised. Name: {e.Name}, Path: {e.FullPath}, Event type: {e.ChangeType}");
        newFileAdded.Set();
      };

      var imagesConversionAndMoveRepeatableProcessor = new ImageSetGetAndSendRepeatableProcessor(_workStopped, sourceRepository, messenger);
      var imageServiceProcessor = new RepeatableWorker(imagesConversionAndMoveRepeatableProcessor, _workStopped, newFileAdded);
      _workingThreads.Add(imageServiceProcessor.GetThread());
      _watchers.Add(watcher);
    }

    
  }
}
