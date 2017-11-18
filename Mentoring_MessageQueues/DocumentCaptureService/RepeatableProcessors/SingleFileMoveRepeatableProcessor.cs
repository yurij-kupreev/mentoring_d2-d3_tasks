using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using DocumentCaptureService.Repositories;
using NLog;

namespace DocumentCaptureService.RepeatableProcessors
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

      await TryToMoveAsync(objectName, 3);

      Logger.Info($"Ended processing object: {objectName}");
    }

    private async Task TryToMoveAsync(string objectName, int tryCount)
    {
      for (var i = 0; i < tryCount; i++) {
        try
        {
          using (var contentStream = _sourceObjectRepository.OpenObjectStream(objectName))
          {
            await _destiantionObjectRepository.SaveObjectAsync(objectName, contentStream);
          }

          _sourceObjectRepository.DeleteObject(objectName);

          return;
        } catch (Exception) {
          Thread.Sleep(5000);
        }
      }
    }
  }
}
