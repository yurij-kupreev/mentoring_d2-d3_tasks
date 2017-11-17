using System.IO;

namespace DocumentCaptureService.Messaging
{
    public interface IMessaging
    {
        void Send(Stream stream, string label);
        Stream Receive();
    }
}