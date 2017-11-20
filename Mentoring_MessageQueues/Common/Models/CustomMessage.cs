using System;
using System.IO;

namespace Common.Models
{
  [Serializable]
  public class CustomMessage
  {
    public string Label { get; set; }
    public Stream Body { get; set; }
  }
}