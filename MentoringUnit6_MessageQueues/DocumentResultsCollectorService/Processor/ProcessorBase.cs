using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DocumentResultsCollectorService.Processor
{
  public abstract class ProcessorBase
  {
    private WaitHandle[] _waitHandle;

    public void SetWaitHandle(ManualResetEvent manualResetEvent, params WaitHandle[] waitHandles)
    {
      var waitHandle = new List<WaitHandle>();

      waitHandle.Add(manualResetEvent);
      waitHandle.AddRange(waitHandles);

      _waitHandle = waitHandle.ToArray();
    }

    public Thread GetThread()
    {
      if (_waitHandle == null || _waitHandle.Length == 0)
      {
        return null;
      }

      return new Thread(WorkProcedure);
    }

    protected void WorkProcedure()
    {
      do
      {
        WorkProcess();
      }
      while (WaitHandle.WaitAny(_waitHandle) != 0);
    }

    public abstract void WorkProcess();
  }
}
