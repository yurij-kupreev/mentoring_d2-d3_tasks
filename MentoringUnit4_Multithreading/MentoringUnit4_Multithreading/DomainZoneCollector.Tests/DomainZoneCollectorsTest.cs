using System;
using System.Threading.Tasks;
using DomainZoneCollector.Collectors;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DomainZoneCollector.Tests
{
  [TestClass]
  public class DomainZoneCollectorsTest
  {
    [TestMethod]
    public void AsyncAwaitCollectorTest()
    {
      var collector = new AsyncAwaitCollector();

      var models = collector.GetDomainZoneModelsAsync().Result;

      var ruZoneModel = models.Find(item => item.Name == ".ru");

      Assert.AreEqual("https://www.iana.org/domains/root/db/ru.html", ruZoneModel.DescriptionUri);
      Assert.AreEqual("whois.tcinet.ru", ruZoneModel.WhoisServer);
    }

    [TestMethod]
    public void TPLCollectorTest()
    {
      var collector = new TPLCollector();

      var models = collector.GetDomainZoneModels();

      var ruZoneModel = models.Find(item => item.Name == ".ru");

      Assert.AreEqual("https://www.iana.org/domains/root/db/ru.html", ruZoneModel.DescriptionUri);
      Assert.AreEqual("whois.tcinet.ru", ruZoneModel.WhoisServer);
    }

    [TestMethod]
    public void ThreadCollectorTest()
    {
      var collector = new ThreadCollector();

      var models = collector.GetDomainZoneModels();

      var ruZoneModel = models.Find(item => item.Name == ".ru");

      Assert.AreEqual("https://www.iana.org/domains/root/db/ru.html", ruZoneModel.DescriptionUri);
      Assert.AreEqual("whois.tcinet.ru", ruZoneModel.WhoisServer);
    }
  }
}
