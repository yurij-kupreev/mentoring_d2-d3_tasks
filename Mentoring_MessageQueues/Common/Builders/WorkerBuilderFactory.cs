namespace Common.Builders
{
  public class WorkerBuilderFactory
  {
    private WorkerBuilderFactory()
    {
    }

    public static WorkersWaitHandlesBuilder Create()
    {
      return new WorkersWaitHandlesBuilder();
    }
  }
}