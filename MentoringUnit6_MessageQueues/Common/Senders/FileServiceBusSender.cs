using AzureServiceBusRepository;
using Common.Models;

namespace Common.Senders
{
  public class FileServiceBusSender : FileSender
  {
    private readonly string _queueName;

    public FileServiceBusSender(string queueName)
    {
      _queueName = queueName;
    }

    public override void SendFile(CustomFile file)
    {
      using (var azureServiceBusRepository = new AzureServiceBusRepository<CustomFile>(_queueName))
      {
        azureServiceBusRepository.SendItem(file);
      }
    }
  }
}
