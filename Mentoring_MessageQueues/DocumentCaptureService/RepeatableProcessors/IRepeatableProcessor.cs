using System.Collections.Generic;
using System.Threading;

namespace DocumentCaptureService.RepeatableProcessors
{
  public interface IRepeatableProcessor
  {
    //WaitHandle WorkStopped { get; set; }

    void RepeatableProcess();
   // IEnumerable<WaitHandle> GetNonStoppedWaitHandles();
  }
}