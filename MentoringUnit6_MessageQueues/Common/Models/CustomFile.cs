using System;
using System.IO;

namespace Common.Models
{
  [Serializable]
  public class CustomFile
  {
    public CustomFile(string fileName, byte[] Content)
    {
      this.FileName = fileName;
      this.Content = Content;
    }

    public string FileName { get; set; }
    public byte[] Content { get; set; }

    public void Save (string destinationDirectory)
    {
      var destinationFilePath = Path.Combine(destinationDirectory, this.FileName);

      if (!Directory.Exists(destinationDirectory))
      {
        Directory.CreateDirectory(destinationDirectory);
      }

      File.WriteAllBytes(destinationFilePath, this.Content);
    }
  }
}
