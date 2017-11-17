using System.Collections.Generic;
using System.IO;
using System.Threading;
using DocumentCaptureService.Repositories;
using NLog;

namespace DocumentCaptureService.RepeatableProcessors
{
  public abstract class ObjectMoveRepeatableProcessor : IRepeatableProcessor
  {
    public WaitHandle WorkStopped { get; set; }

    protected static readonly ILogger Logger = LogManager.GetCurrentClassLogger();

    protected readonly IObjectRepository SourceObjectRepository;
    protected readonly IObjectRepository DestiantionObjectRepository;

    protected ObjectMoveRepeatableProcessor(WaitHandle workStopped, IObjectRepository sourceObjectRepository, IObjectRepository destiantionObjectRepository)
    {
      WorkStopped = workStopped;
      SourceObjectRepository = sourceObjectRepository;
      DestiantionObjectRepository = destiantionObjectRepository;
    }

    public abstract void RepeatableProcess();
  }
}