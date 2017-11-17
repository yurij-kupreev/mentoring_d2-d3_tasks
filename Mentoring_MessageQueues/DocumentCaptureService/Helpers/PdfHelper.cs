using System;
using System.Collections.Generic;
using System.IO;
using DocumentCaptureService.Repositories;
using MigraDoc.DocumentObjectModel;
using MigraDoc.DocumentObjectModel.Shapes;
using MigraDoc.Rendering;

namespace DocumentCaptureService.Helpers
{
  public class PdfHelper
  {
    private readonly LocalStorageRepository _localStorageRepository = new LocalStorageRepository(@".\temp");

    public Stream RenderImageDocumentStream(IEnumerable<Stream> objectStreams)
    {
      var document = new Document();
      var section = document.AddSection();

      foreach (var objectStream in objectStreams)
      {
        var imagePath = this.GetPath(objectStream);

        var img = section.AddImage(imagePath);

        this.ProceedImage(img, document);
        section.AddPageBreak();
      }

      if (section.Elements.Count > 1) {
        section.Elements.RemoveObjectAt(section.Elements.Count - 1);

        var render = new PdfDocumentRenderer { Document = document };
        render.RenderDocument();

        var stream = new MemoryStream();
        render.Save(stream, false);

        return stream;
      }

      return null;
    }

    private void ProceedImage(Shape img, Document document)
    {
      img.RelativeHorizontal = RelativeHorizontal.Page;
      img.RelativeVertical = RelativeVertical.Page;

      img.Top = 0;
      img.Left = 0;

      img.Height = document.DefaultPageSetup.PageHeight;
      img.Width = document.DefaultPageSetup.PageWidth;
    }

    private string GetPath(Stream objectStream)
    {
      var objectName = Guid.NewGuid().ToString();

      _localStorageRepository.SaveObject(objectName, objectStream);

      return _localStorageRepository.GetObjectPath(objectName);
    }
  }
}
