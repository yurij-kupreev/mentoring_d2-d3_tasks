using System.IO;
using System.Text.RegularExpressions;
using System.Threading;

namespace MentoringUnit4_WindowsServices.FileProcessors
{
  class ImagesProcessor : FileProcessor
  {
    private PdfHelper _pdfHelper;

    public ImagesProcessor(string inDirectory, string outDirectory, ManualResetEvent workStopped)
        : base(inDirectory, outDirectory, workStopped)
    {
      _pdfHelper = new PdfHelper();
    }

    protected override void WorkProcess()
    {
      var wasEndImage = false;
      _pdfHelper.CreateNewDocument();
      if (_workStopped.WaitOne(0))
      {
        TrySaveDocument(3);
        return;
      }
      foreach (var filePath in Directory.EnumerateFiles(_inDirectory))
      {
        if (_workStopped.WaitOne(0))
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
      if (wasEndImage) TrySaveDocument(3);
    }

    private bool IsImage(string fileName)
    {
      var pattern = @"[\s\S]*[.](?:png|jpeg|jpg)";
      var regex = new Regex(pattern, RegexOptions.IgnoreCase);

      return regex.IsMatch(fileName);
    }


    //The file should end with "End" substring. "True" result should run pdf save process.
    private bool IsEndImage(string fileName)
    {
      var pattern = @"[\s\S]*End[.](?:png|jpeg|jpg)";
      var regex = new Regex(pattern, RegexOptions.IgnoreCase);

      return regex.IsMatch(fileName);
    }

    public void TrySaveDocument(int tryCount)
    {
      for (var i = 0; i < tryCount; i++)
      {
        try
        {
          _pdfHelper.SaveDocument(_outDirectory);
          return;
        }
        catch (IOException)
        {
          Thread.Sleep(5000);
        }
      }
    }
  }
}
