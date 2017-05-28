using System.IO;
using System.Threading;

namespace MentoringUnit4_WindowsServices
{
  internal class FileProcessor
  {
    public FileSystemWatcher _watcher { get; private set; }

    protected string _inDirectory;
    protected string _outDirectory;
    protected ManualResetEvent _workStopped;
    protected AutoResetEvent _newFileAdded;

    public FileProcessor(string inDirectory, string outDirectory, ManualResetEvent workStopped)
    {
      _inDirectory = inDirectory;
      _outDirectory = outDirectory;

      CreateIfNotExist(_inDirectory);
      CreateIfNotExist(_outDirectory);

      _workStopped = workStopped;
      _newFileAdded = new AutoResetEvent(false);

      _watcher = new FileSystemWatcher(_inDirectory);
      _watcher.Created += On_Created;
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
      while (WaitHandle.WaitAny(new WaitHandle[] { _workStopped, _newFileAdded }) != 0);
    }

    protected virtual void WorkProcess()
    {
      if (_workStopped.WaitOne(0)) return;
      foreach (var filePath in Directory.EnumerateFiles(_inDirectory))
      {
        if (_workStopped.WaitOne(0)) return;
        if (TryToOpen(filePath, 3))
        {
          var fileName = Path.GetFileName(filePath);
          MoveFile(Path.Combine(_inDirectory, fileName), Path.Combine(_outDirectory, fileName));
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

    private static void MoveFile(string sourceFilePath, string newFilePath)
    {
      if (File.Exists(newFilePath))
      {
        File.Delete(newFilePath);
      }

      File.Move(sourceFilePath, newFilePath);
    }

    private void On_Created(object sender, FileSystemEventArgs e)
    {
      _newFileAdded.Set();
    }

    private void CreateIfNotExist(string path)
    {
      if (!Directory.Exists(path))
      {
        Directory.CreateDirectory(path);
      }
    }
  }
}
