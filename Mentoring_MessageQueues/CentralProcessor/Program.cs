using CentralProcessor.Services;
using Topshelf;

namespace CentralProcessor
{
  class Program
  {
    static void Main(string[] args)
    {
      HostFactory.Run(
          hostConf => {
            hostConf.Service<FileCentralProcessorService>(
            s => {
              s.ConstructUsing(() => new FileCentralProcessorService());
              s.WhenStarted(serv => serv.Start());
              s.WhenStopped(serv => serv.Stop());
            });
            hostConf.SetServiceName("FileCentalProcessor_");
            hostConf.SetDisplayName("File Central Processor");
            hostConf.StartManually();
            hostConf.RunAsLocalService();
            hostConf.UseNLog();
          }
      );
    }
  }
}
