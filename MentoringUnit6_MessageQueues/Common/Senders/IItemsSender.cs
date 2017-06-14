namespace Common.Senders
{
  public interface IItemSender<T>
  {
    void SendItem(T item);

    void SendItems(T[] items);
  }
}
