using System;

namespace Common.Models
{
    [Serializable]
    public class ChunkMessage
    {
        public string Id { get; set; }
        public int Position { get; set; }
        public int Count { get; set; }
        public byte[] Content { get; set; }
    }
}