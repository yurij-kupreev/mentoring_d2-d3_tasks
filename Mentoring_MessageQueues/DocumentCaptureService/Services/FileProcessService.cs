using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Threading;
using DocumentCaptureService.RepeatableProcessors;
using DocumentCaptureService.Repositories;
using DocumentCaptureService.ServiceWorkers;

namespace DocumentCaptureService.Services
{
  public class FileProcessService
  {
    private readonly List<Thread> _workingThreads;
    private readonly List<FileSystemWatcher> _watchers;
    private readonly ManualResetEvent _workStopped;

    private const string FilesInputDirectoryKey = "FilesInputDirectory";
    private const string ImagesInputDirectoryKey = "ImagesInputDirectory";
    //private const string FilesOutputDirectoryKey = "FilesOutputDirectory";

    private const string BlobContainerNameKey = "BlobContainerName";
    private const string BlobFolderNameKey = "BlobFolderName";
    private const string AzureStorageConnectionStringKey = "AzureStorageConnectionString";

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

      //var outputDirectory = ConfigurationManager.AppSettings[FilesOutputDirectoryKey];
      //var fileRepository = new LocalStorageRepository(outputDirectory);

      var blobContainerName = ConfigurationManager.AppSettings[BlobContainerNameKey];
      var blobFolderName = ConfigurationManager.AppSettings[BlobFolderNameKey];
      var azureStorageConnectionString = ConfigurationManager.AppSettings[AzureStorageConnectionStringKey];

      var blobStorageRepository = new BlobStorageRepository(blobContainerName, azureStorageConnectionString, blobFolderName);

      var singleFileMoveRepeatableProcessor = new SingleFileMoveRepeatableProcessor(inputDirectory, _workStopped, blobStorageRepository);
      var fileServiceProcessor = new RepeatableWorker(singleFileMoveRepeatableProcessor);
      _workingThreads.Add(fileServiceProcessor.GetThread());
      _watchers.Add(singleFileMoveRepeatableProcessor.Watcher);
    }

    private void InitImageProcessor()
    {
      var imagesDirectory = ConfigurationManager.AppSettings[ImagesInputDirectoryKey];

      var inputDirectory = ConfigurationManager.AppSettings[FilesInputDirectoryKey];  
      var imagesRepository = new LocalStorageRepository(inputDirectory);

      var imagesConversionAndMoveRepeatableProcessor = new ImagesConversionAndMoveRepeatableProcessor(imagesDirectory, _workStopped, imagesRepository);
      var imageServiceProcessor = new RepeatableWorker(imagesConversionAndMoveRepeatableProcessor);
      _workingThreads.Add(imageServiceProcessor.GetThread());
      _watchers.Add(imagesConversionAndMoveRepeatableProcessor.Watcher);
    }
  }
}
