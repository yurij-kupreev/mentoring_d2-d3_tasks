using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace DomainZoneCollector.Collectors
{
  public class AsyncAwaitCollector : DomainZoneCollector
  {
    public override async Task<List<DomainZoneModel>> GetDomainZoneModelsAsync()
    {
      var domainZoneModels = await this.WorkProcess();

      return domainZoneModels;
    }

    private async Task<List<DomainZoneModel>> WorkProcess()
    {
      var domainZoneModels = new List<DomainZoneModel>();

      using (var client = new HttpClient())
      {
        var html = await client.GetStringAsync(RootZoneDbUri);

        domainZoneModels.AddRange(this.GetDomainZoneModelsNonWhoisServer(html));

        await this.PopulateWhoisServers(domainZoneModels, client);
      }

      return domainZoneModels;
    }

    private async Task PopulateWhoisServers(IEnumerable<DomainZoneModel> domainZoneModels, HttpClient client)
    {
      var tasks = domainZoneModels.Select(domainZoneModel => this.PopulateWhoisServer(domainZoneModel, client));

      foreach (var task in tasks)
      {
        await task;
      }
    }

    private async Task PopulateWhoisServer(DomainZoneModel domainZoneModel, HttpClient client)
    {
      var html = await client.GetStringAsync(domainZoneModel.DescriptionUri);

      domainZoneModel.WhoisServer = this.GetWhoisServerLink(html);
    }

    public override List<DomainZoneModel> GetDomainZoneModels()
    {
      throw new System.NotImplementedException();
    }
  }
}