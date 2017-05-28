using System.Diagnostics;
using System.IO;
using NLog;
using NLog.Config;
using NLog.Targets;
using Topshelf;

namespace MentoringUnit4_WindowsServices
{
  internal class Program
  {
    private static void Main(string[] args)
    {
      var currentDir = Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName);
      var inDir = Path.Combine(currentDir, "in");
      var outDir = Path.Combine(currentDir, "out");

      var logConfig = new LoggingConfiguration();
      var target = new FileTarget()
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
            hostConf.Service<FileProcessSevice>(
                      s =>
                      {
                    s.ConstructUsing(() => new FileProcessSevice(inDir, outDir));
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
