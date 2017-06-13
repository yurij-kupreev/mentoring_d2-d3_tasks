using Common.Models;
using System.IO;

namespace Common.Senders
{
  public abstract class FileSender
  {
    public virtual void SendFile(string sourceDirectory, string fileName)
    {
      var sourceFilePath = Path.Combine(sourceDirectory, fileName);

      var fileBytes = File.ReadAllBytes(sourceFilePath);

      var file = new CustomFile(fileName, fileBytes);

      this.SendFile(file);
    }

    public abstract void SendFile(CustomFile file);
  }
}