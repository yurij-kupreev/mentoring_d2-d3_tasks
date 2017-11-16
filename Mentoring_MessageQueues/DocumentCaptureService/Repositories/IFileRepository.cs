using System.IO;
using System.Threading.Tasks;

namespace DocumentCaptureService.Repositories
{
  public interface IFileRepository
  {
    void SaveFile(string fileName, Stream contentStream);
    Task SaveFileAsync(string fileName, Stream contentStream);
  }
}