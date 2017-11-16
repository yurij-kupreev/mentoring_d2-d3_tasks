using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using DocumentCaptureService.Repositories;

namespace DocumentCaptureService.RepeatableProcessors
{
  public class SingleFileMoveRepeatableProcessor : FileRepeatableProcessor
  {
    private readonly IFileRepository _fileRepository;

    public SingleFileMoveRepeatableProcessor(string directory, WaitHandle workStopped, IFileRepository fileRepository)
      : base(directory, workStopped)
    {
      _fileRepository = fileRepository;
    }

    public override void RepeatableProcess()
    {
      var tasks = new List<Task>();

      foreach (var filePath in Directory.EnumerateFiles(SourceDirectory)) {
        if (WorkStopped.WaitOne(0)) break;

        tasks.Add(this.ProcessFile(filePath));
      }

      Task.WaitAll(tasks.ToArray());
    }

    private async Task ProcessFile(string filePath)
    {
      Logger.Info($"Start processing file: {filePath}");

      await TryToMoveAsync(filePath, 3);

      Logger.Info($"Ended processing file: {filePath}");
    }

    private async Task TryToMoveAsync(string filePath, int tryCount)
    {
      for (var i = 0; i < tryCount; i++) {
        try {
          using (var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read)) {
            await _fileRepository.SaveFileAsync(Path.GetFileName(filePath), fileStream);
          }

          File.Delete(filePath);

          return;
        } catch (IOException) {
          Thread.Sleep(5000);
        }
      }
    }
  }
}
