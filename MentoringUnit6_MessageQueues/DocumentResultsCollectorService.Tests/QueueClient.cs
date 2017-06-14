using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using AzureServiceBusRepository;
using System.IO;

namespace DocumentResultsCollectorService.Tests
{
  [TestClass]
  public class QueueClient
  {
    public const string connString = "Endpoint=sb://ykupreyeumentoring.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=hZILZ1Aouo/STInRkeqFAt1rERYV0wps0t9BtPhg+vQ=";
    public const string queueName = "customqueue";

    [TestMethod]
    public void TestMethod1()
    {
      var arr = new byte[1];

      var memoryStream = new MemoryStream(arr);

      Console.WriteLine(memoryStream.Length);

      using (var azureServiceBusRepository = new AzureServiceBusLargeItemRepository<byte[]>(connString, queueName))
      {
        azureServiceBusRepository.SendItem(memoryStream.ToArray());
      }
    }
  }
}
