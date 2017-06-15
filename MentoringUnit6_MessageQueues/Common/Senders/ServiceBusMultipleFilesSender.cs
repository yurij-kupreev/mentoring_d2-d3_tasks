using System.Threading.Tasks;
using AzureServiceBusRepository;
using Common.Models;

namespace Common.Senders
{
  public class ServiceBusMultipleFilesSender : FileSender
  {
    private readonly string _queueName;
    private readonly string _connectionString;

    public ServiceBusMultipleFilesSender(string connectionString, string queueName)
    {
      _queueName = queueName;
      _connectionString = connectionString;
    }

    public override void SendItem(CustomFile file)
    {
      var files = new CustomFile[] { file };

      this.SendItems(files);
    }

    public override async Task SendItemAsync(CustomFile file)
    {
      var files = new CustomFile[] { file };

      await this.SendItemsAsync(files);
    }

    public override void SendItems(CustomFile[] files)
    {
      var filesMessage = new FileMessage
      {
        CustomFiles = files
      };

      using (var azureServiceBusRepository = new AzureServiceBusLargeItemRepository<FileMessage>(_connectionString, _queueName))
      {
        azureServiceBusRepository.SendItemAsync(filesMessage).Wait();
      }
    }

    public override async Task SendItemsAsync(CustomFile[] files)
    {
      var filesMessage = new FileMessage
      {
        CustomFiles = files
      };

      using (var azureServiceBusRepository = new AzureServiceBusLargeItemRepository<FileMessage>(_connectionString, _queueName))
      {
        await azureServiceBusRepository.SendItemAsync(filesMessage);
      }
    }
  }
}
