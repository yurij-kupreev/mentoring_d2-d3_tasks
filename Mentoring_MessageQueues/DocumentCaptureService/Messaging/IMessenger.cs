using System.IO;
using DocumentCaptureService.Models;

namespace DocumentCaptureService.Messaging
{
    public interface IMessenger
    {
        void Send(CustomMessage message);
        CustomMessage Receive();
    }
}