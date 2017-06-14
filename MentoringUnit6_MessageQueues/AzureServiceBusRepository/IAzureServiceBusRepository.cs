using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AzureServiceBusRepository
{
  public interface IAzureServiceBusRepository<T> : IDisposable
  {
    void SendItem(T item);
    T ReceiveItem();
  }
}
