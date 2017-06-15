using System;
using System.IO;
using System.Threading.Tasks;
using Common.Models;

namespace Common.Senders
{
  public class LocalStorageSender : FileSender
  {
    private readonly string _destinationDirectory;

    public LocalStorageSender(string destinationDirectory)
    {
      _destinationDirectory = destinationDirectory;
    }

    public override void SendFile(string filePath)
    {
      var sourceDirectory = Path.GetDirectoryName(filePath);
      var fileName = Path.GetFileName(filePath);

      if (!Directory.Exists(_destinationDirectory))
      {
        Directory.CreateDirectory(_destinationDirectory);
      }

      var destinationFilePath = Path.Combine(_destinationDirectory, fileName);
      var sourceFilePath = Path.Combine(sourceDirectory, fileName);

      if (File.Exists(destinationFilePath))
      {
        File.Delete(destinationFilePath);
      }

      File.Copy(sourceFilePath, destinationFilePath);
    }

    public override void SendItem(CustomFile file)
    {
      file.Save(_destinationDirectory);
    }

    public override Task SendItemAsync(CustomFile items)
    {
      throw new NotImplementedException();
    }

    public override void SendItems(CustomFile[] files)
    {
      foreach (var file in files)
      {
        this.SendItem(file);
      }
    }

    public override Task SendItemsAsync(CustomFile[] items)
    {
      throw new NotImplementedException();
    }
  }
}