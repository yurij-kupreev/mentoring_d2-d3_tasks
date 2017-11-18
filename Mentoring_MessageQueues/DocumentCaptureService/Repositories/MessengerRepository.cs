using System.Collections.Generic;
using System.IO;
using System.Messaging;
using System.Threading.Tasks;
using DocumentCaptureService.Messaging;
using DocumentCaptureService.Models;

namespace DocumentCaptureService.Repositories
{
  public class MessengerRepository : IObjectRepository
  {
    private readonly IMessenger _messanger;

    public MessengerRepository(IMessenger messenger)
    {
      _messanger = messenger;
    }

    public void SaveObject(string objectName, Stream contentStream)
    {
      _messanger.Send(new CustomMessage{ Label = objectName, Body = contentStream });
    }

    public Task SaveObjectAsync(string objectName, Stream contentStream)
    {
      return Task.Factory.StartNew(() => SaveObject(objectName, contentStream));
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