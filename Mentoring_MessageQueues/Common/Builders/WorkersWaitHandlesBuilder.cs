using System.Collections.Generic;
using System.IO;
using System.Threading;
using NLog;
using Timer = System.Timers.Timer;

namespace Common.Builders
{
  public class WorkersWaitHandlesBuilder
  {
    public List<WaitHandle> NonStoppedWaitHandles { get; }
    public WaitHandle WorkStoppedHandle { get; private set; }
    public FileSystemWatcher FileSystemWatcher { get; private set; }
    public Timer Timer { get; private set; }

    protected static readonly ILogger Logger = LogManager.GetCurrentClassLogger();

    internal WorkersWaitHandlesBuilder()
    {
      NonStoppedWaitHandles = new List<WaitHandle>();
    }

    public WorkerProcessorsBuilder WithWorkStoppedHandle(WaitHandle workStoppedWaitHandle)
    {
      WorkStoppedHandle = workStoppedWaitHandle;

      return new WorkerProcessorsBuilder(this);
    }

    public WorkersWaitHandlesBuilder WithFileSystemWatcherNonStoppedWaitHandle(string directory)
    {
      var newFileAdded = new AutoResetEvent(false);
      FileSystemWatcher = new FileSystemWatcher(directory);

      FileSystemWatcher.Created += (sender, e) => {
        Logger.Info($"Create event has been raised. Name: {e.Name}, Path: {e.FullPath}, Event type: {e.ChangeType}");
        newFileAdded.Set();
      };

      NonStoppedWaitHandles.Add(newFileAdded);

      return this;
    }

    public WorkersWaitHandlesBuilder WithTimerTickNonStoppedWaitHandle(double interval = 1000)
    {
      var timerTicked = new AutoResetEvent(false);
      Timer = new Timer { Interval = interval };
      Timer.Elapsed += (sender, e) => {
        Logger.Info("Timer tick");
        timerTicked.Set();
      };

      NonStoppedWaitHandles.Add(timerTicked);

      return this;
    }

    public static WorkersWaitHandlesBuilder Create()
    {
      return new WorkersWaitHandlesBuilder();
    }
  }
}