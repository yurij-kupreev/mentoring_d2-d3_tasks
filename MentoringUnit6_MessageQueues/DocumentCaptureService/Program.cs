using System.Diagnostics;
using System.IO;
using NLog;
using NLog.Config;
using NLog.Targets;
using Topshelf;
using DocumentCaptureService.Services;

namespace DocumentCaptureService
{
  internal class Program
  {
    private static void Main(string[] args)
    {
      var currentDir = Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName);
      var inDir = Path.Combine(currentDir, "in");

      var logConfig = new LoggingConfiguration();
      var target = new FileTarget
      {
        Name = "Default",
        FileName = Path.Combine(currentDir, "log.txt"),
        Layout = "${date} ${message} ${onexception:inner=${exception:format=toString}}"
      };

      logConfig.AddTarget(target);
      logConfig.AddRuleForAllLevels(target);

      var logFactory = new LogFactory(logConfig);

      HostFactory.Run(
          hostConf =>
          {
            hostConf.Service<FileProcessService>(
                      s =>
                      {
                        s.ConstructUsing(() => new FileProcessService(inDir));
                        s.WhenStarted(serv => serv.Start());
                        s.WhenStopped(serv => serv.Stop());
                      }).UseNLog(logFactory);
            hostConf.SetServiceName("FileMoveService_");
            hostConf.SetDisplayName("File Move Service");
            hostConf.StartManually();
            hostConf.RunAsLocalService();
          }
      );
    }
  }
}
