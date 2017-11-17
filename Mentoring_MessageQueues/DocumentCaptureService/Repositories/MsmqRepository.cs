using System.Collections.Generic;
using System.IO;
using System.Messaging;
using System.Threading.Tasks;

namespace DocumentCaptureService.Repositories
{
  public class MsmqRepository : IObjectRepository
  {
    private readonly MessageQueue _queue;

    public MsmqRepository(string queueName)
    {
      _queue = MessageQueue.Exists(queueName) ? new MessageQueue(queueName) : MessageQueue.Create(queueName);
    }

    public void SaveObject(string objectName, Stream contentStream)
    {
      throw new System.NotImplementedException();
    }

    public Task SaveObjectAsync(string objectName, Stream contentStream)
    {
      throw new System.NotImplementedException();
    }

    public Stream OpenObjectStream(string objectName)
    {
      throw new System.NotImplementedException();
    }

    public IEnumerable<string> EnumerateObjects()
    {
      throw new System.NotImplementedException();
    }

    public void DeleteObject(string objectName)
    {
      throw new System.NotImplementedException();
    }
  }
}