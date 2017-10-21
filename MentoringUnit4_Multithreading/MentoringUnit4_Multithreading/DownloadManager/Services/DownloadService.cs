using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace DownloadManager.Services
{
  public class DownloadService
  {
    public async Task<string> Download(string address, CancellationToken token)
    {
      using (var client = new HttpClient())
      {
        string html;

        try
        {
          var responseMessage = await client.GetAsync(address, token);
          html = await responseMessage.Content.ReadAsStringAsync();
        }
        catch (TaskCanceledException)
        {
          html = "Downloading has been cancelled.";
        }
        catch (Exception e)
        {
          html = e.ToString();
        }

        return html;
      }
    }
  }
}