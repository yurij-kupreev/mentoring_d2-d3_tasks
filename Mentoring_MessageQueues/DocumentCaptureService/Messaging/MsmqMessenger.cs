using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Messaging;
using System.Text;
using DocumentCaptureService.Models;

namespace DocumentCaptureService.Messaging
{
  public class MsmqMessenger : IMessenger
  {
    public const int MessageMaxSize = 4096000;

    private readonly MessageQueue _queue;

    public MsmqMessenger(string queueName)
    {
      _queue = MessageQueue.Exists(queueName) ? new MessageQueue(queueName) : MessageQueue.Create(queueName);
      _queue.Formatter = new BinaryMessageFormatter();
    }

    public void Send(CustomMessage message)
    {
      var id = message.Label ?? Guid.NewGuid().ToString();
      var propsSize = this.GetChunkMetadataSize(id);

      var bufferSize = MessageMaxSize - propsSize;
      var buffer = new byte[bufferSize];

      var chunkNumbers = (int)Math.Ceiling((double)message.Body.Length / bufferSize);

      for (var i = 1; i <= chunkNumbers; ++i) {
        var outputStream = new MemoryStream();
        var readed = message.Body.Read(buffer, 0, bufferSize);

        outputStream.Write(buffer, 0, readed);

        var chunk = new ChunkMessage
        {
          Id = id,
          Content = outputStream.ToArray(),
          Position = 1,
          Count = chunkNumbers
        };

        _queue.Send(chunk, id);
      }
    }

    public CustomMessage Receive()
    {
      //var dictionary = new Dictionary<string, List<ChunkMessage>>();
      //var label = string.Empty;
      var chunkedMessage = new ChunkedMessage();

      while (chunkedMessage.Chunks.Count == 0 || chunkedMessage.Chunks.First().Count != chunkedMessage.Chunks.Count)
      {
        this.StartEnumeration(chunkedMessage);
      }

      var body = chunkedMessage.Chunks.OrderBy(chunk => chunk.Position).SelectMany(item => item.Content).ToArray();

      var message = new CustomMessage
      {
        Label = chunkedMessage.Label,
        Body = new MemoryStream(body)
      };

      return message;
    }

    private void StartEnumeration(ChunkedMessage chunkedMessage)
    {
      var enumerator = _queue.GetMessageEnumerator2();

      while (enumerator.MoveNext()) {
        if (chunkedMessage.Label == string.Empty) {
          chunkedMessage.Label = enumerator.Current.Label;
          chunkedMessage.Chunks.Add(this.ReceiveChunk(enumerator.Current.Id));
        } else if (enumerator.Current.Label == chunkedMessage.Label) {
          chunkedMessage.Chunks.Add(this.ReceiveChunk(enumerator.Current.Id));
        }
      }
    }

    private ChunkMessage ReceiveChunk(string id)
    {
      var message = _queue.PeekById(id);
      var body = message.Body;
      var chunk = (ChunkMessage) body;
      _queue.ReceiveById(id);

      return chunk;
    }

    private int GetChunkMetadataSize(string chunkId)
    {
      return typeof(ChunkMessage).GetProperties().Count(item => item.PropertyType == typeof(int)) * sizeof(int) + Encoding.Unicode.GetByteCount(chunkId);
    }

    public class ChunkedMessage
    {
      public ChunkedMessage()
      {
        Label = string.Empty;
        Chunks = new List<ChunkMessage>();
      }

      public string Label { get; set; }

      public List<ChunkMessage> Chunks { get; set; }
    }
  }
}