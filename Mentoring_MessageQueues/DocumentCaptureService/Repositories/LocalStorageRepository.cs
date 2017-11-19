using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace DocumentCaptureService.Repositories
{
  public class LocalStorageRepository : IObjectRepository
  {
    private readonly string _destinationDirectory;

    private readonly int _openFileTryCount;

    public LocalStorageRepository(string destinationDirectory, int openFileTryCount = 3)
    {
      _destinationDirectory = destinationDirectory;
      _openFileTryCount = openFileTryCount >= 1 ? openFileTryCount : 1;

      if (!Directory.Exists(destinationDirectory)) {
        Directory.CreateDirectory(destinationDirectory);
      }
    }

    public void SaveObject(string objectName, Stream contentStream)
    {
      var destinationFilePath = this.GetObjectPath(objectName);

      using (var fileStream = new FileStream(destinationFilePath, FileMode.Create, FileAccess.Write)) {
        contentStream.CopyTo(fileStream);
      }
    }

    public async Task SaveObjectAsync(string objectName, Stream contentStream)
    {
      var destinationFilePath = this.GetObjectPath(objectName);

      using (var fileStream = new FileStream(destinationFilePath, FileMode.Create, FileAccess.Write)) {
        await contentStream.CopyToAsync(fileStream);
      }
    }

    public string GetObjectPath(string objectName)
    {
      var destinationFilePath = Path.Combine(_destinationDirectory, objectName);

      return destinationFilePath;
    }

    public Stream OpenObjectStream(string objectName)
    {
      var destinationFilePath = this.GetObjectPath(objectName);

      for (var i = 0; i < this._openFileTryCount; ++i)
      {
        try
        {
          var fileStream = new FileStream(destinationFilePath, FileMode.Open, FileAccess.Read);

          return fileStream;
        }
        catch (IOException)
        {
          Thread.Sleep(TimeSpan.FromSeconds(2));
        }
      }

      return null;
    }

    public IEnumerable<string> EnumerateObjects()
    {
      return Directory.EnumerateFiles(_destinationDirectory).Select(Path.GetFileName);
    }

    public void DeleteObject(string objectName)
    {
      File.Delete(this.GetObjectPath(objectName));
    }
  }
}