using Common.Models;
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

    public abstract void SendItem(CustomFile file);

    public virtual void SendItems(CustomFile[] files)
    {
      foreach(var item in files)
      {
        this.SendItem(item);
      }
    }
  }
}