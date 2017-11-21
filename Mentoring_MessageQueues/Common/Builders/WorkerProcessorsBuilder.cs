using Common.Messaging;
using Common.RepeatableProcessors;
using Common.RepeatableProcessors.FileProcessors;
using Common.RepeatableProcessors.ImageSetProcessors;
using Common.Repositories;

namespace Common.Builders
{
  public class WorkerProcessorsBuilder
  {
    public WorkersWaitHandlesBuilder WorkersWaitHandlesBuilder { get; }
    public IRepeatableProcessor RepeatableProcessor { get; private set; }

    internal WorkerProcessorsBuilder(WorkersWaitHandlesBuilder workersWaitHandlesBuilder)
    {
      WorkersWaitHandlesBuilder = workersWaitHandlesBuilder;
    }

    public WorkerBuilder WithFileGetAndSendRepeatableProcessor(IObjectRepository sourceObjectRepository, IMessenger destiantionMessenger)
    {
      RepeatableProcessor = new FileGetAndSendRepeatableProcessor(WorkersWaitHandlesBuilder.WorkStoppedHandle, sourceObjectRepository, destiantionMessenger);

      return new WorkerBuilder(this);
    }

    public WorkerBuilder WithFileReceiveAndMoveRepeatableProcessor(IMessenger sourceMessenger, IObjectRepository destinationRepository)
    {
      RepeatableProcessor = new FileReceiveAndMoveRepeatableProcessor(WorkersWaitHandlesBuilder.WorkStoppedHandle, sourceMessenger, destinationRepository);

      return new WorkerBuilder(this);
    }

    public WorkerBuilder WithSingleFileMoveRepeatableProcessor(IObjectRepository sourceObjectRepository, IObjectRepository destinationRepository)
    {
      RepeatableProcessor = new SingleFileMoveRepeatableProcessor(WorkersWaitHandlesBuilder.WorkStoppedHandle, sourceObjectRepository, destinationRepository);

      return new WorkerBuilder(this);
    }

    public WorkerBuilder WithImagesConversionAndMoveRepeatableProcessor(IObjectRepository sourceObjectRepository, IObjectRepository destinationRepository)
    {
      RepeatableProcessor = new ImagesConversionAndMoveRepeatableProcessor(WorkersWaitHandlesBuilder.WorkStoppedHandle, sourceObjectRepository, destinationRepository);

      return new WorkerBuilder(this);
    }
  }
}