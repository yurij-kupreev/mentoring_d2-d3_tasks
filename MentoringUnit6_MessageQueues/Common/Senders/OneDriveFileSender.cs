﻿using Common.Models;
using System;

namespace Common.Senders.SingleFileSender
{
  public class OneDriveFileSender : FileSender
  {
    public override void SendItem(CustomFile file)
    {
      throw new NotImplementedException();
    }

    public override void SendItems(CustomFile[] files)
    {
      throw new NotImplementedException();
    }
  }
}
