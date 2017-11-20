using System.Collections.Generic;
using System.Threading;
using Common.RepeatableProcessors;

namespace Common.ServiceWorkers
{
  public class RepeatableWorker
  {
    private readonly List<WaitHandle> _waitHandles;
    private readonly IRepeatableProcessor _serviceRepeatableProcessor;

    //protected static readonly ILogger Logger = LogManager.GetCurrentClassLogger();

    public RepeatableWorker(IRepeatableProcessor serviceRepeatableProcessor, WaitHandle workStopped, params WaitHandle[] nonStoppedWaiteHandles)
    {
      _serviceRepeatableProcessor = serviceRepeatableProcessor;
      _waitHandles = new List<WaitHandle> { workStopped };
      _waitHandles.AddRange(nonStoppedWaiteHandles);
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

    //protected void AddWaitHandles(params WaitHandle[] handle)
    //{
    //  _waitHandles.AddRange(handle);
    //}
  }
}