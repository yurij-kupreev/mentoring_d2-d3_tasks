using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace DomainZoneCollector.Collectors
{
  public class ThreadCollector : DomainZoneCollector
  {
    public override List<DomainZoneModel> GetDomainZoneModels()
    {
      var domainZoneModels = this.WorkProcess();

      return domainZoneModels;
    }

    private List<DomainZoneModel> WorkProcess()
    {
      var domainZoneModels = new List<DomainZoneModel>();

      var thread = new Thread(() =>
      {
        using (var client = new WebClient())
        {
          var html = client.DownloadString(RootZoneDbUri);

          domainZoneModels.AddRange(this.GetDomainZoneModelsNonWhoisServer(html));

          var domainZoneModelsWithEvents = domainZoneModels.Select(item => new DomainZoneModelWithEvent
          {
            ManualResetEvent = new ManualResetEvent(false),
            DomainZoneModel = item
          }).ToList();

          var manulResetEvents = domainZoneModelsWithEvents.Select(item => item.ManualResetEvent).ToArray();

          foreach (var domainZoneModelWithEvent in domainZoneModelsWithEvents)
          {
            ThreadPool.QueueUserWorkItem(PopulateWhoisServer, domainZoneModelWithEvent);
          }

          WaitHandle.WaitAll(manulResetEvents);
        }
      });

      thread.Start();
      thread.Join();
      

      return domainZoneModels;
    }

    private void PopulateWhoisServer(object state)
    {
      var domainZoneModelWithEvent = (DomainZoneModelWithEvent)state;

      using (var client = new WebClient())
      {
        var html = client.DownloadString(domainZoneModelWithEvent.DomainZoneModel.DescriptionUri);

        domainZoneModelWithEvent.DomainZoneModel.WhoisServer = this.GetWhoisServerLink(html);

        domainZoneModelWithEvent.ManualResetEvent.Set();
      }
    }

    public override Task<List<DomainZoneModel>> GetDomainZoneModelsAsync()
    {
      throw new System.NotImplementedException();
    }
  }

  public class DomainZoneModelWithEvent
  {
    public ManualResetEvent ManualResetEvent { get; set; }

    public DomainZoneModel DomainZoneModel { get; set; }
  }
}