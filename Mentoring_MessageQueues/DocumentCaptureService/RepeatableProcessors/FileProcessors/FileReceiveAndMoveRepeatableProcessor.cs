using System.Threading;
using DocumentCaptureService.Messaging;
using DocumentCaptureService.Repositories;
using NLog;

namespace DocumentCaptureService.RepeatableProcessors.FileProcessors
{
  public class FileReceiveAndMoveRepeatableProcessor: IRepeatableProcessor
  {
    public WaitHandle WorkStopped { get; set; }

    private static readonly ILogger Logger = LogManager.GetCurrentClassLogger();

    private readonly IMessenger _sourceMessenger;
    private readonly IObjectRepository _destiantionObjectRepository;

    public FileReceiveAndMoveRepeatableProcessor(WaitHandle workStopped, IMessenger sourceMessenger, IObjectRepository destiantionObjectRepository)
    {
      WorkStopped = workStopped;
      _sourceMessenger = sourceMessenger;
      _destiantionObjectRepository = destiantionObjectRepository;
    }

    public void RepeatableProcess()
    {
      var message = _sourceMessenger.Receive();

      Logger.Info($"Message received. Message label: {message.Label}");

      _destiantionObjectRepository.SaveObject(message.Label, message.Body);

      Logger.Info($"File {message.Label} has been moved to the approriate repository");
    }
  }
}