using System.IO;
using System.Threading.Tasks;

namespace DocumentCaptureService.Repositories
{
  public class MsmqRepository : IFileRepository
  {
    public MsmqRepository(string queueName)
    {
      
    }

    public void SaveFile(string fileName, Stream contentStream)
    {
      throw new System.NotImplementedException();
    }

    public Task SaveFileAsync(string fileName, Stream contentStream)
    {
      throw new System.NotImplementedException();
    }
  }
}