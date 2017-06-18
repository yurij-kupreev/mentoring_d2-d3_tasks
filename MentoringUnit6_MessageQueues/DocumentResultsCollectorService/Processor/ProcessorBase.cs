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
    protected WaitHandle[] _waitHandles;
    protected List<WaitHandle> _customHandles = new List<WaitHandle>();

    public void SetWaitHandle(ManualResetEvent manualResetEvent)
    {
      var waitHandles = new List<WaitHandle>();

      waitHandles.Add(manualResetEvent);
      waitHandles.AddRange(_customHandles);

      _waitHandles = waitHandles.ToArray();
    }

    public Thread GetThread()
    {
      if (_waitHandles == null || _waitHandles.Length == 0)
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
      while (WaitHandle.WaitAny(_waitHandles) != 0);

      this.OnStopping();
    }

    public abstract void OnStopping();

    public abstract void WorkProcess();
  }
}
