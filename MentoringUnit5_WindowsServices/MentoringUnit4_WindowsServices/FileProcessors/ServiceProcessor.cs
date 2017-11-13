using System.Collections.Generic;
using System.Threading;

namespace MentoringUnit4_WindowsServices.FileProcessors
{
  public abstract class ServiceProcessor
  {
    private readonly List<WaitHandle> _waitHandles;

    protected ServiceProcessor(WaitHandle workStopped)
    {
      _waitHandles = new List<WaitHandle> { workStopped };
    }

    public Thread GetThread()
    {
      return new Thread(WorkProcedure);
    }

    private void WorkProcedure()
    {
      do {
        WorkProcess();
      }
      while (WaitHandle.WaitAny(_waitHandles.ToArray()) != 0);
    }

    protected abstract void WorkProcess();

    protected void AddWaitHandles(params WaitHandle[] handle)
    {
      _waitHandles.AddRange(handle);
    }
  }
}