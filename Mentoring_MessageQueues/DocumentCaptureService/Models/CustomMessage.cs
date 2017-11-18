using System;
using System.IO;

namespace DocumentCaptureService.Models
{
  [Serializable]
  public class CustomMessage
  {
    public string Label { get; set; }
    public Stream Body { get; set; }
  }
}