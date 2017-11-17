using System;
using System.IO;
using System.Linq;
using System.Messaging;
using System.Text;
using DocumentCaptureService.Models;

namespace DocumentCaptureService.Messaging
{
    public class MsmqMessaging : IMessaging
    {
        public const int MessageMaxSize = 4096000;

        private readonly MessageQueue _queue;

        public MsmqMessaging(string queueName)
        {
            _queue = MessageQueue.Exists(queueName) ? new MessageQueue(queueName) : MessageQueue.Create(queueName);
        }

        public void Send(Stream stream, string label)
        {
            var id = label ?? Guid.NewGuid().ToString();
            var propsSize = this.GetChunkMetadataSize(id);

            var bufferSize = MessageMaxSize - propsSize;
            var buffer = new byte[bufferSize];

            var chunkNumbers = (int)Math.Ceiling((double)stream.Length / bufferSize);

            for (var i = 1; i <= chunkNumbers; ++i)
            {
                var outputStream = new MemoryStream();
                var readed = stream.Read(buffer, 0, bufferSize);

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

        public Stream Receive()
        {
            throw new System.NotImplementedException();
        }

        private void SendChunks(int chunksMetadataSize, string id)
        {
            
        }

        private int GetChunkMetadataSize(string chunkId)
        {
            return typeof(ChunkMessage).GetProperties().Count(item => item.PropertyType == typeof(int)) * sizeof(int) + Encoding.Unicode.GetByteCount(chunkId);
        }
    }
}