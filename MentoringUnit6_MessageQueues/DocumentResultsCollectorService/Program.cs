using Common.Senders;
using DocumentResultsCollectorService.Processor;
using DocumentResultsCollectorService.Services;
using NLog;
using NLog.Config;
using NLog.Targets;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Topshelf;

namespace DocumentResultsCollectorService
{
  public class Program
  {
    private const string AzureServiceBusConnectionString = "Endpoint=sb://ykupreyeu-mq.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=SsdVnoHssUXJvirdr7H7NpHTKB+vCDRtwdVWB40mHQs=";
    private const string QueueName = "testqueue";

    static void Main(string[] args)
    {
      var currentDir = Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName);
      var outDir = Path.Combine(currentDir, "out");

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


      var serviceBusFileManager = new ServiceBusMultipleFilesManager(AzureServiceBusConnectionString, QueueName);
      var fileSender = new LocalStorageSender(Path.Combine(currentDir, outDir));

      var resultsCollectorProcessor = new ResultsCollectorProcessor(serviceBusFileManager, fileSender);

      HostFactory.Run(
          hostConf =>
          {
            hostConf.Service<ServiceBase>(
                      s =>
                      {
                        s.ConstructUsing(() => new ServiceBase(resultsCollectorProcessor));
                        s.WhenStarted(serv => serv.Start());
                        s.WhenStopped(serv => serv.Stop());
                      }).UseNLog(logFactory);
            hostConf.SetServiceName("CentralService_");
            hostConf.SetDisplayName("Central Service");
            hostConf.StartManually();
            hostConf.RunAsLocalService();
          }
      );
    }
  }
}
