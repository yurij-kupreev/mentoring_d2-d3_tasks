using System.IO;
using System.Threading;
using NLog;

namespace MentoringUnit4_WindowsServices.FileProcessors
{
  public abstract class FileProcessor : ServiceProcessor
  {
    protected static readonly ILogger Logger = LogManager.GetCurrentClassLogger();
    public FileSystemWatcher Watcher { get; }

    protected string SourceDirectory;
    protected WaitHandle WorkStopped;
    protected AutoResetEvent NewFileAdded;

    protected FileProcessor(string directory, WaitHandle workStopped): base(workStopped)
    {
      WorkStopped = workStopped;

      SourceDirectory = directory;
      CreateIfNotExist(SourceDirectory);

      NewFileAdded = new AutoResetEvent(false);
      base.AddWaitHandles(NewFileAdded);

      Watcher = new FileSystemWatcher(SourceDirectory);
      Watcher.Created += On_Created;
    }

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
      NewFileAdded.Set();
    }
  }
}