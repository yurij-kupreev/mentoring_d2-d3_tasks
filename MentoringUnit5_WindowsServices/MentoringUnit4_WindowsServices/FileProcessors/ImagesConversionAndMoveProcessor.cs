using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using MentoringUnit4_WindowsServices.Helpers;
using MentoringUnit4_WindowsServices.Repositories;

namespace MentoringUnit4_WindowsServices.FileProcessors
{
  public class ImagesConversionAndMoveProcessor : FileProcessor
  {
    private readonly PdfHelper _pdfHelper;

    private const string ImageFileNamePattern = @"[\s\S]*[.](?:png|jpeg|jpg)";
    private const string EndImageFileNamePattern = @"[\s\S]*End[.](?:png|jpeg|jpg)";

    private readonly IFileRepository _fileRepository;

    public ImagesConversionAndMoveProcessor(string sourceDirectory, WaitHandle workStopped, IFileRepository fileRepository)
        : base(sourceDirectory, workStopped)
    {
      _fileRepository = fileRepository;
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
        var imageFileNames = string.Join(", ", Directory.EnumerateFiles(SourceDirectory).Select(Path.GetFileName));
        Logger.Info($"Start image conversion to pdf file: {imageFileNames}");
        TrySaveDocument(3);
        Logger.Info("Ended image conversion to pdf file and saving.");
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
          var contentStream = _pdfHelper.SaveDocument();
          var pdfFileName = $"images_{DateTime.Now:MM-dd-yy_H-mm-ss}.pdf";

          _fileRepository.SaveFile(pdfFileName, contentStream);

          return;
        }
        catch (IOException)
        {
          Thread.Sleep(5000);
        }
      }
    }

    private bool TryToOpen(string filePath, int tryCount)
    {
      for (var i = 0; i < tryCount; i++) {
        try {
          var file = File.Open(filePath, FileMode.Open, FileAccess.Read, FileShare.None);
          file.Close();

          return true;
        } catch (IOException) {
          Thread.Sleep(5000);
        }
      }

      return false;
    }
  }
}
