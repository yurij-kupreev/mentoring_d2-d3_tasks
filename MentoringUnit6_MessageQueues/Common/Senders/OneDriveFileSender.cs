using Common.Models;
using System;
using System.Threading.Tasks;

namespace Common.Senders.SingleFileSender
{
  public class OneDriveFileSender : FileSender
  {
    public override void SendItem(CustomFile file)
    {
      throw new NotImplementedException();
    }

    public override Task SendItemAsync(CustomFile items)
    {
      throw new NotImplementedException();
    }

    public override void SendItems(CustomFile[] files)
    {
      throw new NotImplementedException();
    }

    public override Task SendItemsAsync(CustomFile[] items)
    {
      throw new NotImplementedException();
    }
  }
}
