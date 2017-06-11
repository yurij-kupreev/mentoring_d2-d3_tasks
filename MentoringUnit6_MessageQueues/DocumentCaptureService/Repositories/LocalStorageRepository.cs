using System.IO;

namespace DocumentCaptureService.Repositories
{
  public class LocalStorageRepository : IFileRepository
  {
    private readonly string _destinationDirectory;

    public LocalStorageRepository(string destinationDirectory)
    {
      _destinationDirectory = destinationDirectory;
    }

    public void MoveFile(string sourceDirectory, string fileName)
    {
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

      File.Move(sourceFilePath, destinationFilePath);
    }
  }
}