using System.IO;
using System.Threading;
using MentoringUnit4_WindowsServices.Repositories;

namespace MentoringUnit4_WindowsServices.FileProcessors
{
  public class FileProcessor
  {
    public FileSystemWatcher Watcher { get; }

    protected string SourceDirectory;
    protected ManualResetEvent WorkStopped;
    protected AutoResetEvent NewFileAdded;

    protected readonly IFileRepository FileRepository;

    public FileProcessor(string directory, ManualResetEvent workStopped, IFileRepository fileRepository)
    {
      FileRepository = fileRepository;

      SourceDirectory = directory;

      CreateIfNotExist(SourceDirectory);

      WorkStopped = workStopped;

      NewFileAdded = new AutoResetEvent(false);

      Watcher = new FileSystemWatcher(SourceDirectory);
      Watcher.Created += On_Created;
    }

    public Thread GetThread()
    {
      return new Thread(WorkProcedure);
    }

    protected void WorkProcedure()
    {
      do
      {
        WorkProcess();
      }
      while (WaitHandle.WaitAny(new WaitHandle[] { WorkStopped, NewFileAdded }) != 0);
    }

    protected virtual void WorkProcess()
    {
      if (WorkStopped.WaitOne(0)) return;

      foreach (var filePath in Directory.EnumerateFiles(SourceDirectory))
      {
        if (WorkStopped.WaitOne(0)) return;

        if (TryToOpen(filePath, 3))
        {
          var fileName = Path.GetFileName(filePath);
          FileRepository.MoveFile(SourceDirectory, fileName);
        }
      }
    }

    protected bool TryToOpen(string filePath, int tryCount)
    {
      for (var i = 0; i < tryCount; i++)
      {
        try
        {
          var file = File.Open(filePath, FileMode.Open, FileAccess.Read, FileShare.None);
          file.Close();

          return true;
        }
        catch (IOException)
        {
          Thread.Sleep(5000);
        }
      }

      return false;
    }

    private void On_Created(object sender, FileSystemEventArgs e)
    {
      NewFileAdded.Set();
    }

    protected void CreateIfNotExist(string path)
    {
      if (!Directory.Exists(path))
      {
        Directory.CreateDirectory(path);
      }
    }
  }
}
