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
  public class ImagesConversionAndMoveRepeatableProcessor : ObjectMoveRepeatableProcessor
  {
    private readonly PdfHelper _pdfHelper;

    private const string ImageFileNamePattern = @"[\s\S]*[.](?:png|jpeg|jpg)";
    private const string EndImageFileNamePattern = @"[\s\S]*End[.](?:png|jpeg|jpg)";

    public ImagesConversionAndMoveRepeatableProcessor(WaitHandle workStopped, IObjectRepository sourceObjectRepository, IObjectRepository destiantionObjectRepository)
        : base(workStopped, sourceObjectRepository, destiantionObjectRepository)
    {
      _pdfHelper = new PdfHelper();
    }

    public override void RepeatableProcess()
    {
      var wasEndImage = false;
      var imageObjectNames = new List<string>();

      foreach (var objectName in SourceObjectRepository.EnumerateObjects()) {
        if (WorkStopped.WaitOne(0)) {
          if (wasEndImage) TrySaveDocument(3, imageObjectNames);
          return;
        }

        if (IsImage(objectName) && TryToOpen(objectName, 3)) {
          wasEndImage = wasEndImage | IsEndImage(objectName);
          imageObjectNames.Add(objectName);
        }
      }

      if (wasEndImage) {
        TrySaveDocument(3, imageObjectNames);
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

    private void TrySaveDocument(int tryCount, IEnumerable<string> imageObjectNames)
    {
      Logger.Info($"Start image conversion to pdf file: {string.Join(", ", imageObjectNames)}");
      for (var i = 0; i < tryCount; i++) {
        try {
          var contentStream = _pdfHelper.RenderImageDocumentStream(imageObjectNames.Select(objectName => SourceObjectRepository.OpenObjectStream(objectName)));
          var pdfFileName = $"images_{DateTime.Now:MM-dd-yy_H-mm-ss}.pdf";

          DestiantionObjectRepository.SaveObject(pdfFileName, contentStream);

          Logger.Info("Ended image conversion to pdf file and saving.");
          return;
        } catch (Exception) {
          Thread.Sleep(5000);
        }
      }

    }

    private bool TryToOpen(string objectName, int tryCount)
    {
      for (var i = 0; i < tryCount; i++) {
        try
        {
          var objectStream = SourceObjectRepository.OpenObjectStream(objectName);
          objectStream.Close();

          return true;
        } catch (Exception) {
          Thread.Sleep(5000);
        }
      }

      return false;
    }
  }
}
