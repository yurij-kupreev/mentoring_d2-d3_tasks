using System.Collections.Generic;
using System.IO;
using System.Threading;
using NLog;

namespace DocumentCaptureService.RepeatableProcessors
{
  public abstract class FileRepeatableProcessor : IRepeatableProcessor
  {
    public WaitHandle WorkStopped { get; set; }

    public FileSystemWatcher Watcher { get; }

    protected static readonly ILogger Logger = LogManager.GetCurrentClassLogger();
    protected string SourceDirectory;
    private readonly AutoResetEvent _newFileAdded;

    protected FileRepeatableProcessor(string directory, WaitHandle workStopped)
    {
      WorkStopped = workStopped;

      SourceDirectory = directory;
      CreateIfNotExist(SourceDirectory);

      _newFileAdded = new AutoResetEvent(false);

      Watcher = new FileSystemWatcher(SourceDirectory);
      Watcher.Created += On_Created;
    }

    public abstract void RepeatableProcess();

    protected void CreateIfNotExist(string path)
    {
      if (!Directory.Exists(path)) {
        Directory.CreateDirectory(path);
        Logger.Info($"Create directory {path}.");
      }
    }

    private void On_Created(object sender, FileSystemEventArgs e)
    {
      Logger.Info($"Create event has been raised. Name: {e.Name}, Path: {e.FullPath}, Event type: {e.ChangeType}");
      _newFileAdded.Set();
    }

    public IEnumerable<WaitHandle> GetNonStoppedWaitHandles()
    {
      return new List<WaitHandle> { _newFileAdded };
    }
  }
}