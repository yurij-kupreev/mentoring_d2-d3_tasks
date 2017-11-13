using System.Collections.Generic;
using System.IO;
using MigraDoc.DocumentObjectModel;
using MigraDoc.DocumentObjectModel.Shapes;
using MigraDoc.Rendering;

namespace MentoringUnit4_WindowsServices.Helpers
{
  public class PdfHelper
  {
    private Document _currentDocument;
    private Section _currentSection;

    public List<string> Images { get; private set; }

    public void AddImage(string filePath)
    {
      var img = _currentSection.AddImage(filePath);

      this.ProceedImage(img);

      _currentSection.AddPageBreak();

      Images.Add(filePath);
    }

    private void ProceedImage(Image img)
    {
      img.RelativeHorizontal = RelativeHorizontal.Page;
      img.RelativeVertical = RelativeVertical.Page;

      img.Top = 0;
      img.Left = 0;

      img.Height = _currentDocument.DefaultPageSetup.PageHeight;
      img.Width = _currentDocument.DefaultPageSetup.PageWidth;
    }

    public Stream SaveDocument()
    {
      if (_currentDocument != null && Images.Count > 0) {
        var render = new PdfDocumentRenderer();
        render.Document = _currentDocument;
        render.RenderDocument();

        var stream = new MemoryStream();
        render.Save(stream, false);

        _currentDocument = null;

        return stream;
      }

      return null;
    }

    public void CreateNewDocument()
    {
      _currentDocument = new Document();
      _currentSection = _currentDocument.AddSection();
      Images = new List<string>();
    }
  }
}
