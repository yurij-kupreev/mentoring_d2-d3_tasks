using System;
using System.Threading;
using Common.Messaging;
using Common.Models;
using Common.Repositories;
using NLog;

namespace Common.RepeatableProcessors.FileProcessors
{
  public class FileReceiveAndMoveRepeatableProcessor: IRepeatableProcessor
  {
    public WaitHandle WorkStopped { get; set; }

    private static readonly ILogger Logger = LogManager.GetCurrentClassLogger();

    private readonly IMessenger _sourceMessenger;
    private readonly IObjectRepository _destiantionObjectRepository;

    private readonly ProcessorStatus _processorStatus;

    public FileReceiveAndMoveRepeatableProcessor(WaitHandle workStopped, IMessenger sourceMessenger, IObjectRepository destiantionObjectRepository)
    {
      WorkStopped = workStopped;
      _sourceMessenger = sourceMessenger;
      _destiantionObjectRepository = destiantionObjectRepository;

      _processorStatus = new ProcessorStatus()
      {
        SourceName = this.GetType().Name,
        ProcessorStartTime = DateTime.Now
      };
    }

    public void RepeatableProcess()
    {
      var message = _sourceMessenger.Receive();

      Logger.Info($"Message received. Message label: {message.Label}");

      _destiantionObjectRepository.SaveObject(message.Label, message.Body);

      Logger.Info($"File {message.Label} has been moved to the approriate repository");

      _processorStatus.ProcessedObjects.Add(message.Label);
    }

    public ProcessorStatus GetProcessorStatus()
    {
      return _processorStatus;
    }
  }
}