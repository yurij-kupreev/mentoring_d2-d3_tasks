namespace Common.RepeatableProcessors
{
  public interface IRepeatableProcessor
  {
    //WaitHandle WorkStopped { get; set; }

    void RepeatableProcess();
   // IEnumerable<WaitHandle> GetNonStoppedWaitHandles();
  }
}