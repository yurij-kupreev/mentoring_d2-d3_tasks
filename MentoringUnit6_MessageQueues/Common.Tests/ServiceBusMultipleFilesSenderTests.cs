using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Common.Models;
using System.Linq;
using Common.Senders;
using AzureServiceBusRepository;
using System.Collections.Generic;
using System.IO;

namespace Common.Tests
{
  [TestClass]
  public class ServiceBusMultipleFilesSenderTests
  {
    private const string AzureServiceBusConnectionString = "Endpoint=sb://ykupreyeu-mq.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=SsdVnoHssUXJvirdr7H7NpHTKB+vCDRtwdVWB40mHQs=";
    private const string QueueName = "testqueue";

    private const string TestPath = @"F:\Dev\epam mentoring d2-d3\mentoring tasks\MentoringUnit6_MessageQueues\Common.Tests\bin\Debug\test\";

    [TestMethod]
    public void LargeFileSendReceiveTest()
    {
      var contentSize = 470000;

      var customFile = this.GenerateLargeTestFile(contentSize);

      IItemSender<CustomFile> serviceBusMultipleFilesSender = new ServiceBusMultipleFilesManager(AzureServiceBusConnectionString, QueueName);

      serviceBusMultipleFilesSender.SendItemAsync(customFile).Wait();

      CustomFile newCustomFile;

      using (var azureServiceBusRepository = new AzureServiceBusLargeItemRepository<FileMessage>(AzureServiceBusConnectionString, QueueName))
      {
        newCustomFile = azureServiceBusRepository.ReceiveItemAsync().Result.CustomFiles[0];
      }

      Assert.AreEqual(customFile.FileName, newCustomFile.FileName);
      Assert.IsTrue(Enumerable.SequenceEqual(customFile.Content, newCustomFile.Content));
    }

    [TestMethod]
    public void FileSetSendReceiveTest()
    {
      var contentSizes = new int[] { 100000, 50000, 200000 };
      var customFileList = new List<CustomFile>();

      foreach (var contentSize in contentSizes)
      {
        customFileList.Add(this.GenerateLargeTestFile(contentSize));
      }

      var customFiles = customFileList.ToArray();

      IItemSender<CustomFile> serviceBusMultipleFilesSender = new ServiceBusMultipleFilesManager(AzureServiceBusConnectionString, QueueName);

      serviceBusMultipleFilesSender.SendItemsAsync(customFiles).Wait();

      CustomFile[] newCustomFiles;

      using (var azureServiceBusRepository = new AzureServiceBusLargeItemRepository<FileMessage>(AzureServiceBusConnectionString, QueueName))
      {
        newCustomFiles = azureServiceBusRepository.ReceiveItemAsync().Result.CustomFiles;
      }

      foreach (var newCustomFile in newCustomFiles)
      {
        var index = Array.IndexOf(newCustomFiles, newCustomFile);

        Assert.AreEqual(customFiles[index].FileName, newCustomFile.FileName);
        Assert.IsTrue(Enumerable.SequenceEqual(customFiles[index].Content, newCustomFile.Content));
      }
    }

    public CustomFile GenerateLargeTestFile(int contentSize)
    {
      var fileContent = new byte[contentSize];

      var random = new Random();
      random.NextBytes(fileContent);

      var customFile = new CustomFile(Guid.NewGuid().ToString() + ".test", fileContent);

      return customFile;
    }

    [TestMethod]
    public void ReceiveFileTest()
    {
      CustomFile[] newCustomFiles;

      using (var azureServiceBusRepository = new AzureServiceBusLargeItemRepository<FileMessage>(AzureServiceBusConnectionString, QueueName))
      {
        newCustomFiles = azureServiceBusRepository.ReceiveItemAsync().Result.CustomFiles;
      }

      foreach (var newCustomFile in newCustomFiles)
      {
        File.WriteAllBytes(TestPath + newCustomFile.FileName, newCustomFile.Content);
      }
    }
  }
}
