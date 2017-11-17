using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace DocumentCaptureService.Repositories
{
  public class LocalStorageRepository : IObjectRepository
  {
    private readonly string _destinationDirectory;

    public LocalStorageRepository(string destinationDirectory)
    {
      _destinationDirectory = destinationDirectory;

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

      var fileStream = new FileStream(destinationFilePath, FileMode.Open, FileAccess.Read);

      return fileStream;
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