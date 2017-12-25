using System.Collections.Generic;
using System.IO;
using MigraDoc.DocumentObjectModel;
using MigraDoc.DocumentObjectModel.Shapes;
using MigraDoc.Rendering;

namespace MentoringUnit4_WindowsServices.Helpers
{
  public class PdfHelper
  {
    public Stream RenderDocumentStream(IEnumerable<string> imagePaths)
    {
      var document = new Document();
      var section = document.AddSection();

      foreach (var imagePath in imagePaths) {
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
  }
}
