using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace DocumentCaptureService.Repositories
{
  public interface IObjectRepository
  {
    void SaveObject(string objectName, Stream contentStream);
    Task SaveObjectAsync(string objectName, Stream contentStream);
    Stream OpenObjectStream(string objectName);
    IEnumerable<string> EnumerateObjects();
    void DeleteObject(string objectName);

  }
}