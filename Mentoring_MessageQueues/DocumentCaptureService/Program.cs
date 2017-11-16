using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DocumentCaptureService.Services;
using Topshelf;

namespace DocumentCaptureService
{
  class Program
  {
    static void Main(string[] args)
    {
      HostFactory.Run(
          hostConf => {
            hostConf.Service<FileProcessService>(
            s => {
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
