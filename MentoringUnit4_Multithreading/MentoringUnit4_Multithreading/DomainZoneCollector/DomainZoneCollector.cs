using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using CsQuery;
using CsQuery.ExtensionMethods;

namespace DomainZoneCollector
{
  public abstract class DomainZoneCollector
  {
    public const string BaseUri = "https://www.iana.org";
    protected const string RootZoneDbPath = "/domains/root/db";
    protected string RootZoneDbUri = BaseUri + RootZoneDbPath;

    public abstract List<DomainZoneModel> GetDomainZoneModels();
    public abstract Task<List<DomainZoneModel>> GetDomainZoneModelsAsync();

    protected IEnumerable<DomainZoneModel> GetDomainZoneModelsNonWhoisServer(string html)
    {
      var domainATags = this.GetDomainZoneATagsDomObject(html);

      var domainZoneModels = domainATags.Select(item => new DomainZoneModel
      {
        Name = item.InnerText,
        DescriptionUri = BaseUri + item.GetAttribute("href")
      }).Skip(1050).Take(20).ToList();

      return domainZoneModels;
    }

    protected string GetWhoisServerLink(string html)
    {
      var whois = this.GetWhoisDomObject(html);

      if (whois != null)
      {
        var nodeValue = whois.NextSibling.NodeValue;
        var whoisServerLink = Regex.Replace(nodeValue, @"\r\n?|\n", string.Empty).Trim();

        return whoisServerLink;
      }

      return string.Empty;
    }

    private IEnumerable<IDomObject> GetDomainZoneATagsDomObject(string html)
    {
      var cq = CQ.Create(html);

      var domainATags = cq[".domain"][".tld"]["a"].Where(item => item.GetAttribute("href").Contains(RootZoneDbPath));

      return domainATags;
    }

    private IDomObject GetWhoisDomObject(string html)
    {
      var cq = CQ.Create(html);

      var whois = cq[":contains(WHOIS Server)"].Last().FirstOrDefault();

      return whois;
    }
  }
}
