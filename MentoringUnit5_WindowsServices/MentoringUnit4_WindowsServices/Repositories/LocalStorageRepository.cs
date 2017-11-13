using System.IO;
using System.Threading.Tasks;

namespace MentoringUnit4_WindowsServices.Repositories
{
  public class LocalStorageRepository : IFileRepository
  {
    private readonly string _destinationDirectory;

    public LocalStorageRepository(string destinationDirectory)
    {
      _destinationDirectory = destinationDirectory;
    }

    public void SaveFile(string fileName, Stream contentStream)
    {
      var destinationFilePath = this.PrepareFileDestination(fileName);

      using (var fileStream = new FileStream(destinationFilePath, FileMode.Create, FileAccess.Write)) {
        contentStream.CopyTo(fileStream);
      }
    }

    public async Task SaveFileAsync(string fileName, Stream contentStream)
    {
      var destinationFilePath = this.PrepareFileDestination(fileName);

      using (var fileStream = new FileStream(destinationFilePath, FileMode.Create, FileAccess.Write)) {
        await contentStream.CopyToAsync(fileStream);
      }
    }

    private string PrepareFileDestination(string fileName)
    {
      if (!Directory.Exists(_destinationDirectory)) {
        Directory.CreateDirectory(_destinationDirectory);
      }

      var destinationFilePath = Path.Combine(_destinationDirectory, fileName);

      return destinationFilePath;
    }
  }
}