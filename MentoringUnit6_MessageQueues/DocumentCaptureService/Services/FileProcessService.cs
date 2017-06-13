using System.Collections.Generic;
using System.IO;
using System.Threading;
using DocumentCaptureService.FileProcessors;
using Common.Senders;

namespace DocumentCaptureService.Services
{
  public class FileProcessService
  {
    private readonly List<Thread> _workingThreads;
    private readonly List<FileSystemWatcher> _watchers;
    private readonly ManualResetEvent _workStopped;

    public FileProcessService(string inDirectory)
    {
      var imagesDirectory = Path.Combine(inDirectory, "images");

      var currentDirectory = Path.GetDirectoryName(inDirectory);

      var outDirectory = Path.Combine(currentDirectory, "out");

      _workStopped = new ManualResetEvent(false);

      _workingThreads = new List<Thread>();
      _watchers = new List<FileSystemWatcher>();

      var fileSender = new LocalStorageSender(outDirectory);
      
      var fileProcessor = new FileProcessor(inDirectory, _workStopped, fileSender);
      _workingThreads.Add(fileProcessor.GetThread());
      _watchers.Add(fileProcessor.Watcher);

      var imageProcessor = new ImagesProcessor(imagesDirectory, _workStopped, fileSender);
      _workingThreads.Add(imageProcessor.GetThread());
      _watchers.Add(imageProcessor.Watcher);
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
  }
}
