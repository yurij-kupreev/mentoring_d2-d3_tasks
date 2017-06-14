using AzureServiceBusRepository;
using Common.Models;

namespace Common.Senders
{
  public class ServiceBusMultipleFilesSender : FileSender
  {
    private readonly string _queueName;

    public ServiceBusMultipleFilesSender(string queueName)
    {
      _queueName = queueName;
    }

    public override void SendItem(CustomFile file)
    {
      var files = new CustomFile[] { file };

      this.SendItems(files);
    }

    public void SendItem(CustomFile[] files)
    {
      var filesMessage = new FilesMessage
      {
        CustomFiles = files
      };

      using (var azureServiceBusRepository = new AzureServiceBusLargeItemRepository<FilesMessage>(_queueName))
      {
        azureServiceBusRepository.SendItem(filesMessage);
      }
    }
  }

  public class FilesMessage
  {
    public CustomFile[] CustomFiles;
  }
}
