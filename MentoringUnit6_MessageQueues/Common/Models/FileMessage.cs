using System;

namespace Common.Models
{
  [Serializable]
  public class FileMessage
  {
    public CustomFile[] CustomFiles;

    public FileMessageType FilesMessageType => CustomFiles?.Length > 1 ? FileMessageType.ImageSet : FileMessageType.File;
  }

  public enum FileMessageType
  {
    File = 0,
    ImageSet = 1
  }
}
