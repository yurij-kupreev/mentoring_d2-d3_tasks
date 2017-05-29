using System.Collections.Generic;
using System.IO;
using System.Threading;
using MentoringUnit4_WindowsServices.FileProcessors;

namespace MentoringUnit4_WindowsServices
{
  public class FileProcessSevice
  {
    private readonly string _inDirectory;
    private readonly string _outDirectory;
    private readonly string _imagesDirectory;

    private List<Thread> _workingThreads;
    private List<FileSystemWatcher> _watchers;
    private ManualResetEvent _workStopped;

    public FileProcessSevice(string inDirectory, string outDirectory)
    {
      _inDirectory = inDirectory;
      _outDirectory = outDirectory;
      _imagesDirectory = Path.Combine(_inDirectory, "images");

      _workStopped = new ManualResetEvent(false);

      _workingThreads = new List<Thread>();
      _watchers = new List<FileSystemWatcher>();

      var fileProcessor = new FileProcessor(_inDirectory, _outDirectory, _workStopped);
      _workingThreads.Add(fileProcessor.GetThread());
      _watchers.Add(fileProcessor._watcher);

      var imageProcessor = new ImagesProcessor(_imagesDirectory, _outDirectory, _workStopped);
      _workingThreads.Add(imageProcessor.GetThread());
      _watchers.Add(imageProcessor._watcher);
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
