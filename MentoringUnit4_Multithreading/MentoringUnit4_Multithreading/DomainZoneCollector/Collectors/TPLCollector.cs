using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

namespace DomainZoneCollector.Collectors
{
  public class TPLCollector : DomainZoneCollector
  {
    public override List<DomainZoneModel> GetDomainZoneModels()
    {
      var domainZoneModels = WorkProcess();

      return domainZoneModels;
    }

    public List<DomainZoneModel> WorkProcess()
    {
      var domainZoneModels = new List<DomainZoneModel>();

      using (var client = new WebClient())
      {
        Task<List<DomainZoneModel>>.Factory.StartNew(() =>
        {
          var html = client.DownloadString(RootZoneDbUri);
          domainZoneModels.AddRange(this.GetDomainZoneModelsNonWhoisServer(html));

          this.PopulateWhoisServers(domainZoneModels);

          return domainZoneModels;
        }).Wait();
      }
      
      return domainZoneModels;
    }

    private void PopulateWhoisServers(IEnumerable<DomainZoneModel> domainZoneModels)
    {
      Parallel.ForEach(domainZoneModels, item =>
      {
        using (var client = new WebClient())
        {
          var html = client.DownloadString(item.DescriptionUri);
          item.WhoisServer = this.GetWhoisServerLink(html);
        }
      });
    }

    public override Task<List<DomainZoneModel>> GetDomainZoneModelsAsync()
    {
      throw new System.NotImplementedException();
    }
  }
}