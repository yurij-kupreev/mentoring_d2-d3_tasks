using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using DocumentCaptureService.Repositories;

namespace DocumentCaptureService.RepeatableProcessors
{
  public class SingleFileMoveRepeatableProcessor : ObjectMoveRepeatableProcessor
  {
    public SingleFileMoveRepeatableProcessor(WaitHandle workStopped, IObjectRepository sourceObjectRepository, IObjectRepository destiantionObjectRepository)
      : base(workStopped, sourceObjectRepository, destiantionObjectRepository)
    {
    }

    public override void RepeatableProcess()
    {
      var tasks = new List<Task>();

      foreach (var objectName in SourceObjectRepository.EnumerateObjects()) {
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
          using (var contentStream = SourceObjectRepository.OpenObjectStream(objectName))
          {
            await DestiantionObjectRepository.SaveObjectAsync(objectName, contentStream);
          }

          SourceObjectRepository.DeleteObject(objectName);

          return;
        } catch (Exception) {
          Thread.Sleep(5000);
        }
      }
    }
  }
}
