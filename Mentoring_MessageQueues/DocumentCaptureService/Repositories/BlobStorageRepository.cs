using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;

namespace DocumentCaptureService.Repositories
{
  public class BlobStorageRepository: IObjectRepository
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

    public void SaveObject(string objectName, Stream contentStream)
    {
      var blobStreamPath = GetBlobPath(objectName);
      var blobToStream = GetBlobForStreaming(blobStreamPath);
      var cloudBlobStream = blobToStream.OpenWrite();

      using (cloudBlobStream)
      {
        contentStream.CopyTo(cloudBlobStream);
      }
    }

    public async Task SaveObjectAsync(string objectName, Stream contentStream)
    {
      var blobStreamPath = GetBlobPath(objectName);
      var blobToStream = GetBlobForStreaming(blobStreamPath);
      var cloudBlobStream = await blobToStream.OpenWriteAsync();

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

    public Stream OpenObjectStream(string objectName)
    {
      var blobStreamPath = GetBlobPath(objectName);
      var blobToStream = GetBlobForStreaming(blobStreamPath);
      return blobToStream.OpenRead();
    }

    public IEnumerable<string> EnumerateObjects()
    {
      var directory = _container.GetDirectoryReference(_folderPath);
      var blobs = directory.ListBlobs();

      var filePathes = blobs.OfType<CloudBlob>().Select(b => b.Name);

      var fileNames = filePathes.Select(this.GetFileName).ToList();

      return fileNames;
    }

    private string GetFileName(string filePath)
    {
      var folderPathCharactersNumber = (_folderPath + "\\").Length;

      if (filePath == null || folderPathCharactersNumber > filePath.Length) {
        return string.Empty;
      }

      var fileName = filePath.Substring(folderPathCharactersNumber);

      return fileName;
    }

    public void DeleteObject(string objectName)
    {
      var blobStreamPath = GetBlobPath(objectName);

      _container.GetBlobReference(blobStreamPath).DeleteIfExists();
    }
  }
}