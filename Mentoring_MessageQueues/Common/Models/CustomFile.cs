using System;

namespace Common.Models
{
    [Serializable]
    public class CustomFile
    {
        public string FileName { get; set; }
        public byte[] Content { get; set; }
    }
}