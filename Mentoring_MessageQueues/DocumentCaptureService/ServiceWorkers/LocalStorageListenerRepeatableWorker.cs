using System.ComponentModel;
using System.IO;
using System.Threading;
using DocumentCaptureService.RepeatableProcessors;

namespace DocumentCaptureService.ServiceWorkers
{
  //public class LocalStorageListenerRepeatableWorker : RepeatableWorker
  //{
  //  public FileSystemWatcher Watcher { get; }
  //  private readonly AutoResetEvent _newFileAdded;

  //  public LocalStorageListenerRepeatableWorker(IRepeatableProcessor serviceRepeatableProcessor, WaitHandle workStopped): this()
  //  {
  //    _newFileAdded = new AutoResetEvent(false);

  //    Watcher = new FileSystemWatcher(SourceDirectory);
  //    Watcher.Created += On_Created;
  //  }

  //  private LocalStorageListenerRepeatableWorker(IRepeatableProcessor serviceRepeatableProcessor, WaitHandle workStopped, params WaitHandle[] nonStoppedWaiteHandles)
  //    : base(serviceRepeatableProcessor, workStopped, nonStoppedWaiteHandles)
  //  {
      
  //  }

  //  private void On_Created(object sender, FileSystemEventArgs e)
  //  {
  //    Logger.Info($"Create event has been raised. Name: {e.Name}, Path: {e.FullPath}, Event type: {e.ChangeType}");
  //    _newFileAdded.Set();
  //  }
  //}
}