using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using AzureServiceBusRepository;

namespace DocumentResultsCollectorService.Tests
{
  [TestClass]
  public class QueueClient
  {
    public const string connString = "Endpoint=sb://mentoringkupreyeu.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=dwNagJMbNPnldV7RWtmiCk6zzAr1ZSHGzqz0YR8sh1w=";
    public const string queueName = "MentoringDocumentQueue";

    [TestMethod]
    public void TestMethod1()
    {
      using (var azureServiceBusRepository = new AzureServiceBusRepository<string>(connString, queueName))
      {
        azureServiceBusRepository.SendItem("Hello queue!");
      }
    }
  }
}
