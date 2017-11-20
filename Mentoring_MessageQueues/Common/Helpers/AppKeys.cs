namespace Common.Helpers
{
  public class AppKeys
  {
    public const string FilesInputDirectoryKey = "FilesInputDirectory";
    public const string ImagesInputDirectoryKey = "ImagesInputDirectory";
    public const string FilesOutputDirectoryKey = "FilesOutputDirectory";

    public const string BlobContainerNameKey = "BlobContainerName";
    public const string BlobFolderNameKey = "BlobFolderName";
    public const string AzureStorageConnectionStringKey = "AzureStorageConnectionString";

    public const string MsmqSingleFileQueueNameKey = "MsmqSingleFileQueueName";
    public const string MsmqImageSetQueueNameKey = "MsmqImageSetQueueName";

    public const string AzureServiceBusConnectionStringKey = "AzureServiceBusConnectionString";
    public const string StatusTopicNameKey = "StatusTopicName";
    public const string StatusSubscriptionNameKey = "StatusSubscriptionName";

    public const string StatusResponseTopicNameKey = "StatusResponseTopicName";
    public const string StatusResponseSubscriptionNameKey = "StatusResponseSubscriptionName";
  }
}