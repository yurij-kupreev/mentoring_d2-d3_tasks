using System.IO;
using System.Threading;
using MentoringUnit4_WindowsServices.Repositories;

namespace MentoringUnit4_WindowsServices.RepeatableProcessors
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
      if (WorkStopped.WaitOne(0)) return;

      foreach (var filePath in Directory.EnumerateFiles(SourceDirectory)) {
        Logger.Info($"Start processing file: {filePath}");

        TryToMove(filePath, 3);

        Logger.Info($"Ended processing file: {filePath}");
      }
    }

    private void TryToMove(string filePath, int tryCount)
    {
      for (var i = 0; i < tryCount; i++) {
        try {
          using (var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read)) {
            _fileRepository.SaveFile(Path.GetFileName(filePath), fileStream);
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
