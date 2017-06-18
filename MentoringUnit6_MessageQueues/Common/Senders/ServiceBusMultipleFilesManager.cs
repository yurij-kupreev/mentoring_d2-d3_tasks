using System.Threading.Tasks;
using AzureServiceBusRepository;
using Common.Models;
using Common.Getters;
using System;

namespace Common.Senders
{
  public class ServiceBusMultipleFilesManager : FileSender, IItemGet<FileMessage>
  {
    private readonly string _queueName;
    private readonly string _connectionString;

    public ServiceBusMultipleFilesManager(string connectionString, string queueName)
    {
      _queueName = queueName;
      _connectionString = connectionString;
    }

    public async Task<FileMessage> GetItemAsync()
    {
      FileMessage item = null;

      using (var azureServiceBusRepository = new AzureServiceBusLargeItemRepository<FileMessage>(_connectionString, _queueName))
      {
        item = await azureServiceBusRepository.ReceiveItemAsync();
      }

      return item;
      //throw new NotImplementedException();
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
