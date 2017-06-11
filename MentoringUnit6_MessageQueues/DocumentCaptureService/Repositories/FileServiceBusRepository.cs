using AzureServiceBusRepository;
using System.IO;

namespace DocumentCaptureService.Repositories
{
  public class FileServiceBusRepository : IFileRepository
  {
    private readonly string _queueName;

    public FileServiceBusRepository(string queueName)
    {
      _queueName = queueName;
    }

    public void MoveFile(string sourceDirectory, string fileName)
    {
      var sourceFilePath = Path.Combine(sourceDirectory, fileName);

      var fileBytes = File.ReadAllBytes(sourceFilePath);

      var customFile = new CustomFile
      {
        FileName = fileName,
        Content = fileBytes
      };

      using (var azureServiceBusRepository = new AzureServiceBusRepository<CustomFile>(_queueName))
      {
        azureServiceBusRepository.SendItem(customFile);
      }
    }
  }

  public class CustomFile
  {
    public string FileName { get; set; }
    public byte[] Content { get; set; }
  }
}
