using DocumentResultsCollectorService.Processor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DocumentResultsCollectorService.Services
{
  public class CentralService: ServiceBase
  {
    public CentralService(params ProcessorBase[] processors)
    {
      foreach(var processor in processors)
      {
        this.AddProcessor(processor);
      }
    }
  }
}
