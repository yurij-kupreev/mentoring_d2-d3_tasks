using Common.Models;

namespace Common.RepeatableProcessors
{
  public interface IRepeatableProcessor
  {
    //WaitHandle WorkStopped { get; set; }

    void RepeatableProcess();

    ProcessorStatus GetProcessorStatus();
    // IEnumerable<WaitHandle> GetNonStoppedWaitHandles();
  }
}