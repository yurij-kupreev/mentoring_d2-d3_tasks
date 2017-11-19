using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using DocumentCaptureService.Repositories;
using NLog;

namespace DocumentCaptureService.RepeatableProcessors.FileProcessors
{
  public class SingleFileMoveRepeatableProcessor : IRepeatableProcessor
  {
    public WaitHandle WorkStopped { get; set; }

    private static readonly ILogger Logger = LogManager.GetCurrentClassLogger();

    private readonly IObjectRepository _sourceObjectRepository;
    private readonly IObjectRepository _destiantionObjectRepository;

    public SingleFileMoveRepeatableProcessor(WaitHandle workStopped, IObjectRepository sourceObjectRepository, IObjectRepository destiantionObjectRepository)
    {
      WorkStopped = workStopped;
      _sourceObjectRepository = sourceObjectRepository;
      _destiantionObjectRepository = destiantionObjectRepository;
    }

    public void RepeatableProcess()
    {
      var tasks = new List<Task>();

      foreach (var objectName in _sourceObjectRepository.EnumerateObjects()) {
        if (WorkStopped.WaitOne(0)) break;

        tasks.Add(this.ProcessFile(objectName));
      }

      Task.WaitAll(tasks.ToArray());
    }

    private async Task ProcessFile(string objectName)
    {
      Logger.Info($"Start processing object: {objectName}");

      await MoveAsync(objectName);

      Logger.Info($"Ended processing object: {objectName}");
    }

    private async Task MoveAsync(string objectName)
    {
      using (var contentStream = _sourceObjectRepository.OpenObjectStream(objectName)) {
        await _destiantionObjectRepository.SaveObjectAsync(objectName, contentStream);
      }

      _sourceObjectRepository.DeleteObject(objectName);
    }

  }
}
