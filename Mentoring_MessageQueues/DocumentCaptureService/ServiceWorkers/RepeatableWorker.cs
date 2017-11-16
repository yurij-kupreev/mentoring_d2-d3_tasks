using System.Collections.Generic;
using System.Threading;
using DocumentCaptureService.RepeatableProcessors;

namespace DocumentCaptureService.ServiceWorkers
{
  public class RepeatableWorker
  {
    private readonly List<WaitHandle> _waitHandles;
    private readonly IRepeatableProcessor _serviceRepeatableProcessor;

    public RepeatableWorker(IRepeatableProcessor serviceRepeatableProcessor)
    {
      _serviceRepeatableProcessor = serviceRepeatableProcessor;
      _waitHandles = new List<WaitHandle> { _serviceRepeatableProcessor.WorkStopped };
      _waitHandles.AddRange(_serviceRepeatableProcessor.GetNonStoppedWaitHandles());
    }

    public Thread GetThread()
    {
      return new Thread(WorkProcedure);
    }

    private void WorkProcedure()
    {
      do {
        _serviceRepeatableProcessor.RepeatableProcess();
      }
      while (WaitHandle.WaitAny(_waitHandles.ToArray()) != 0);
    }

    protected void AddWaitHandles(params WaitHandle[] handle)
    {
      _waitHandles.AddRange(handle);
    }
  }
}