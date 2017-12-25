using System.Collections.Generic;
using System.Threading;

namespace MentoringUnit4_WindowsServices.RepeatableProcessors
{
  public interface IRepeatableProcessor
  {
    WaitHandle WorkStopped { get; set; }

    void RepeatableProcess();
    IEnumerable<WaitHandle> GetNonStoppedWaitHandles();
  }
}