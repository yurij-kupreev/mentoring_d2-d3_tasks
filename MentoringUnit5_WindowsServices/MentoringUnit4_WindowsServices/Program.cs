using MentoringUnit4_WindowsServices.Services;
using Topshelf;

namespace MentoringUnit4_WindowsServices
{
  internal class Program
  {
    private static void Main(string[] args)
    {
      HostFactory.Run(
          hostConf =>
          {
            hostConf.Service<FileProcessService>(
            s =>
            {
                s.ConstructUsing(() => new FileProcessService());
                s.WhenStarted(serv => serv.Start());
                s.WhenStopped(serv => serv.Stop());
            });
            hostConf.SetServiceName("FileMoveService_");
            hostConf.SetDisplayName("File Move Service");
            hostConf.StartManually();
            hostConf.RunAsLocalService();
            hostConf.UseNLog();
          }
      );
    }
  }
}
