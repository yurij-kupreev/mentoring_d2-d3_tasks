using DocumentResultsCollectorService.Processor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DocumentResultsCollectorService.Services
{
  public class ServiceBase
  {
    protected readonly List<Thread> _workingThreads = new List<Thread>();
    protected readonly ManualResetEvent _workStopped = new ManualResetEvent(false);

    public ServiceBase(params ProcessorBase[] processors)
    {
      foreach (var processor in processors)
      {
        this.AddProcessor(processor);
      }
    }

    public ServiceBase AddProcessor(ProcessorBase processor)
    {
      processor.SetWaitHandle(_workStopped);
      _workingThreads.Add(processor.GetThread());

      return this;
    }

    public virtual void Start()
    {
      _workStopped.Reset();
      _workingThreads.ForEach(thr => thr.Start());
    }

    public virtual void Stop()
    {
      _workStopped.Set();

      _workingThreads.ForEach(thr => thr.Join());
    }
  }
}
