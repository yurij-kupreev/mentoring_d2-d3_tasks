﻿using System.Threading.Tasks;

namespace Common.Senders
{
  public interface IItemSender<T>
  {
    void SendItem(T item);
    void SendItems(T[] items);

    Task SendItemAsync(T item);
    Task SendItemsAsync(T[] items); 
  }
}