using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using DocumentCaptureService.Helpers;
using DocumentCaptureService.Repositories;

namespace DocumentCaptureService.RepeatableProcessors
{
  public class ImagesConversionAndMoveRepeatableProcessor : FileRepeatableProcessor
  {
    private readonly PdfHelper _pdfHelper;

    private const string ImageFileNamePattern = @"[\s\S]*[.](?:png|jpeg|jpg)";
    private const string EndImageFileNamePattern = @"[\s\S]*End[.](?:png|jpeg|jpg)";

    private readonly IFileRepository _fileRepository;

    public ImagesConversionAndMoveRepeatableProcessor(string sourceDirectory, WaitHandle workStopped, IFileRepository fileRepository)
        : base(sourceDirectory, workStopped)
    {
      _fileRepository = fileRepository;
      _pdfHelper = new PdfHelper();
    }

    public override void RepeatableProcess()
    {
      var wasEndImage = false;
      var imagePaths = new List<string>();

      foreach (var filePath in Directory.EnumerateFiles(SourceDirectory)) {
        if (WorkStopped.WaitOne(0)) {
          if (wasEndImage) TrySaveDocument(3, imagePaths);
          return;
        }

        if (IsImage(filePath) && TryToOpen(filePath, 3)) {
          wasEndImage = wasEndImage | IsEndImage(filePath);
          imagePaths.Add(filePath);
        }
      }

      if (wasEndImage) {
        TrySaveDocument(3, imagePaths);
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

    private void TrySaveDocument(int tryCount, IEnumerable<string> imagePaths)
    {
      var imageFileNames = string.Join(", ", Directory.EnumerateFiles(SourceDirectory).Select(Path.GetFileName));
      Logger.Info($"Start image conversion to pdf file: {imageFileNames}");
      for (var i = 0; i < tryCount; i++) {
        try {
          var contentStream = _pdfHelper.RenderDocumentStream(imagePaths);
          var pdfFileName = $"images_{DateTime.Now:MM-dd-yy_H-mm-ss}.pdf";

          _fileRepository.SaveFile(pdfFileName, contentStream);

          Logger.Info("Ended image conversion to pdf file and saving.");
          return;
        } catch (IOException) {
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
