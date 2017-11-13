using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Threading;
using MentoringUnit4_WindowsServices.FileProcessors;
using MentoringUnit4_WindowsServices.Repositories;

namespace MentoringUnit4_WindowsServices.Services
{
  public class FileProcessService
  {
    private readonly List<Thread> _workingThreads;
    private readonly List<FileSystemWatcher> _watchers;
    private readonly ManualResetEvent _workStopped;

    private const string FilesInputDirectoryKey = "FilesInputDirectory";
    private const string ImagesInputDirectoryKey = "ImagesInputDirectory";
    private const string FilesOutputDirectoryKey = "FilesOutputDirectory";

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
      var fileRepository = new LocalStorageRepository(outputDirectory);

      var fileProcessor = new SingleFileMoveProcessor(inputDirectory, _workStopped, fileRepository);
      _workingThreads.Add(fileProcessor.GetThread());
      _watchers.Add(fileProcessor.Watcher);
    }

    private void InitImageProcessor()
    {
      var imagesDirectory = ConfigurationManager.AppSettings[ImagesInputDirectoryKey];

      var outputDirectory = ConfigurationManager.AppSettings[FilesOutputDirectoryKey];  
      var imagesRepository = new LocalStorageRepository(outputDirectory);

      var imageProcessor = new ImagesConversionAndMoveProcessor(imagesDirectory, _workStopped, imagesRepository);
      _workingThreads.Add(imageProcessor.GetThread());
      _watchers.Add(imageProcessor.Watcher);
    }
  }
}
