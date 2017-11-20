using Common.Models;

namespace Common.Messaging
{
    public interface IMessenger
    {
        void Send(CustomMessage message);
        CustomMessage Receive();
    }
}