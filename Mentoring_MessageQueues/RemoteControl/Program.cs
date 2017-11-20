using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RemoteControl.Services;
using Topshelf;

namespace RemoteControl
{
  class Program
  {
    static void Main(string[] args)
    {
      HostFactory.Run(
          hostConf => {
            hostConf.Service<RemoteControlService>(
            s => {
              s.ConstructUsing(() => new RemoteControlService());
              s.WhenStarted(serv => serv.Start());
              s.WhenStopped(serv => serv.Stop());
            });
            hostConf.SetServiceName("RemoteControl_");
            hostConf.SetDisplayName("File Processor remote control");
            hostConf.StartManually();
            hostConf.RunAsLocalService();
            hostConf.UseNLog();
          }
      );
    }
  }
}
