using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Common.RepeatableProcessors;
using Common.ServiceWorkers;

namespace Common.Builders
{
  public class WorkerBuilder
  {
    public IRepeatableProcessor RepeatableProcessor => WorkerProcessorsBuilder.RepeatableProcessor;
    public WaitHandle WorkStoppedHandle => WorkerProcessorsBuilder.WorkersWaitHandlesBuilder.WorkStoppedHandle;
    private IEnumerable<WaitHandle> NonStoppedWaitHandles => WorkerProcessorsBuilder.WorkersWaitHandlesBuilder.NonStoppedWaitHandles;

    internal WorkerBuilder()
    {
    }

    public WorkerProcessorsBuilder WorkerProcessorsBuilder { get; }
    public WorkerBuilder(WorkerProcessorsBuilder workerProcessorsBuilder)
    {
      WorkerProcessorsBuilder = workerProcessorsBuilder;
    }

    public RepeatableWorker Build()
    {
      return new RepeatableWorker(
        RepeatableProcessor,
        WorkStoppedHandle,
        NonStoppedWaitHandles.ToArray());
    }
  }
}