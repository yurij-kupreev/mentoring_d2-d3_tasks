using Common.Getters;
using Common.Helpers;
using Common.Models;
using Common.Senders;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace DocumentResultsCollectorService.Processor
{
  public class ResultsCollectorProcessor : ProcessorBase
  {
    private AutoResetEvent _resetEvent;
    private IItemGet<FileMessage> _itemGetter;
    private FileSender _fileSender;

    private List<Task> _tasks = new List<Task>();

    public ResultsCollectorProcessor(IItemGet<FileMessage> itemGetter, FileSender fileSender)
    {
      _itemGetter = itemGetter;
      _fileSender = fileSender;

      _resetEvent = new AutoResetEvent(false);
      this._customHandles.Add(_resetEvent);
    }

    public override void WorkProcess()
    {
      try
      {
        var item = _itemGetter.GetItemAsync().Result;
        _resetEvent.Set();
        _tasks.Add(this.Proceed(item));
      }
      catch (TimeoutException e)
      {
        _resetEvent.Set();
      }
    }

    public async Task Proceed(FileMessage fileMessage)
    {
      switch (fileMessage.FilesMessageType)
      {
        case FileMessageType.File:
          foreach (var file in fileMessage.CustomFiles)
          {
            await this.ProceedFile(file);
          }
          break;
        case FileMessageType.ImageSet:
          await this.ProceedImageSet(fileMessage.CustomFiles);
          break;
      }
    }

    public override void OnStopping()
    {
      Task.WaitAll(_tasks.ToArray());
    }

    public async Task ProceedFile(CustomFile customFile)
    {
      await _fileSender.SendItemAsync(customFile);
    }

    public async Task ProceedImageSet(CustomFile[] customFiles)
    {
      var helper = new PdfHelper();
      helper.CreateNewDocument();

      for (var i = 0; i < customFiles.Length - 1; ++i)
      {
        helper.AddImage(customFiles[i].Content);
      }

      helper.AddImage(customFiles[customFiles.Length - 1].Content, true);

      var pdfFile = helper.SaveDocument();

      await this.ProceedFile(pdfFile);
    }
  }
}
