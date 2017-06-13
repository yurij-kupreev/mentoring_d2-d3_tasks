using Common.Helpers;
using Common.Senders;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading;

namespace DocumentCaptureService.FileProcessors
{
  public class ImagesProcessor : FileProcessor
  {
    private readonly PdfHelper _pdfHelper;

    private const string ImageFileNamePattern = @"[\s\S]*[.](?:png|jpeg|jpg)";
    private const string EndImageFileNamePattern = @"[\s\S]*End[.](?:png|jpeg|jpg)";

    public ImagesProcessor(string sourceDirectory, ManualResetEvent workStopped, FileSender fileRepository)
        : base(sourceDirectory, workStopped, fileRepository)
    {
       _pdfHelper = new PdfHelper();
    }

    protected override void WorkProcess()
    {
      var wasEndImage = false;
      _pdfHelper.CreateNewDocument();
      if (WorkStopped.WaitOne(0))
      {
        TrySaveDocument(3);
        return;
      }
      foreach (var filePath in Directory.EnumerateFiles(SourceDirectory))
      {
        if (WorkStopped.WaitOne(0))
        {
          if (wasEndImage) TrySaveDocument(3);
          return;
        }

        if (IsImage(filePath) && TryToOpen(filePath, 3))
        {
          wasEndImage = wasEndImage | IsEndImage(filePath);
          _pdfHelper.AddImage(filePath);
        }
      }

      if (wasEndImage)
      {
        TrySaveDocument(3);
      }
    }

    private bool IsImage(string fileName)
    {
      var regex = new Regex(ImageFileNamePattern, RegexOptions.IgnoreCase);

      return regex.IsMatch(fileName);
    }


    //The file should end with "End" substring. "True" result should run pdf save process.
    private bool IsEndImage(string fileName)
    {
      var regex = new Regex(EndImageFileNamePattern, RegexOptions.IgnoreCase);

      return regex.IsMatch(fileName);
    }

    public void TrySaveDocument(int tryCount)
    {
      for (var i = 0; i < tryCount; i++)
      {
        try
        {
          var customFile = _pdfHelper.SaveDocument();

          if (customFile != null)
          {
            _fileRepository.SendFile(customFile);

            return;
          }
        }
        catch (IOException)
        {
          Thread.Sleep(5000);
        }
      }
    }
  }
}
