using Common.Models;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Common.Senders
{
  public abstract class FileSender : IItemSender<CustomFile>
  {
    public virtual void SendFile(string filePath)
    {
      var file = this.GetCustomFile(filePath);

      this.SendItem(file);
    }

    public virtual async Task SendFileAsync(string filePath)
    {
      var file = this.GetCustomFile(filePath);

      await this.SendItemAsync(file);
    }

    protected CustomFile GetCustomFile(string filePath)
    {
      var fileName = Path.GetFileName(filePath);

      var fileBytes = File.ReadAllBytes(filePath);

      var file = new CustomFile(fileName, fileBytes);

      return file;
    }

    public virtual void SendFiles(string[] filePaths)
    {
      var files = this.GetCustomFileArray(filePaths);

      this.SendItems(files);
    }

    public virtual async Task SendFilesAsync(string[] filePaths)
    {
      var files = this.GetCustomFileArray(filePaths);

      await this.SendItemsAsync(files);
    }

    protected CustomFile[] GetCustomFileArray(string[] filePaths)
    {
      var files = new List<CustomFile>();

      foreach (var filePath in filePaths)
      {
        var fileBytes = File.ReadAllBytes(filePath);
        var fileName = Path.GetFileName(filePath);

        var file = new CustomFile(fileName, fileBytes);

        files.Add(file);
      }

      return files.ToArray();
    }

    public abstract void SendItem(CustomFile file);

    public abstract Task SendItemAsync(CustomFile items);

    public abstract void SendItems(CustomFile[] files);

    public abstract Task SendItemsAsync(CustomFile[] items);
  }
}