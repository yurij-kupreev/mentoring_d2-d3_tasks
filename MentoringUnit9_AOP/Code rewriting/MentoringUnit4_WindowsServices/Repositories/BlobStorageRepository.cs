using System.IO;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;

namespace MentoringUnit4_WindowsServices.Repositories
{
  public class BlobStorageRepository: IFileRepository
  {
    private readonly string _folderPath;
    private readonly CloudBlobContainer _container;

    internal string ContainerName;

    public BlobStorageRepository(string containerName, string connectionString, string folderPath)
    {
      _folderPath = folderPath;
      ContainerName = containerName;

      var storageAccount = CloudStorageAccount.Parse(connectionString);

      var blobClient = storageAccount.CreateCloudBlobClient();
      _container = blobClient.GetContainerReference(ContainerName);
      _container.CreateIfNotExists();
    }

    public void SaveFile(string fileName, Stream contentStream)
    {
      var blobStreamPath = GetBlobPath(fileName);
      var blobToStream = GetBlobForStreaming(blobStreamPath);
      var cloudBlobStream = blobToStream.OpenWrite();

      using (cloudBlobStream)
      {
        contentStream.CopyTo(cloudBlobStream);
      }
    }

    public async Task SaveFileAsync(string fileName, Stream contentStream)
    {
      var blobStreamPath = GetBlobPath(fileName);
      var blobToStream = GetBlobForStreaming(blobStreamPath);
      var cloudBlobStream = blobToStream.OpenWrite();

      using (cloudBlobStream) {
        await contentStream.CopyToAsync(cloudBlobStream);
      }
    }

    private string GetBlobPath(string objectName)
    {
      if (_folderPath == string.Empty) return objectName;

      return _folderPath + "\\" + objectName;
    }

    private CloudBlockBlob GetBlobForStreaming(string blobStreamPath)
    {
      var blobToStream = _container.GetBlockBlobReference(blobStreamPath);

      // according to azure documentation, this prop must be set to a min of 16K for streaming operations to work properly
      blobToStream.StreamMinimumReadSizeInBytes = 16384;

      return blobToStream;
    }
  }
}