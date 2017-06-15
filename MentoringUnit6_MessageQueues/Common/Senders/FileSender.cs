using Common.Models;
using System.Collections.Generic;
using System.IO;

namespace Common.Senders
{
  public abstract class FileSender : IItemSender<CustomFile>
  {
    public virtual void SendFile(string sourceDirectory, string fileName)
    {
      var sourceFilePath = Path.Combine(sourceDirectory, fileName);

      var fileBytes = File.ReadAllBytes(sourceFilePath);

      var file = new CustomFile(fileName, fileBytes);

      this.SendItem(file);
    }

    public virtual void SendFiles(string[] filePaths)
    {
      var files = new List<CustomFile>();

      foreach (var filePath in filePaths)
      {
        var fileBytes = File.ReadAllBytes(filePath);
        var fileName = Path.GetFileName(filePath);

        var file = new CustomFile(fileName, fileBytes);

        files.Add(file);
      }

      this.SendItems(files.ToArray());
    }

    public abstract void SendItem(CustomFile file);

    public abstract void SendItems(CustomFile[] files);
  }
}