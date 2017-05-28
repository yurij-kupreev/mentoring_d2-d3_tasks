using System;
using System.Collections.Generic;
using System.IO;
using MigraDoc.DocumentObjectModel;
using MigraDoc.DocumentObjectModel.Shapes;
using MigraDoc.Rendering;

namespace MentoringUnit4_WindowsServices
{
  public class PdfHelper
  {
    private Document _currentDocument;
    private Section _currentSection;

    public List<string> Images { get; private set; }

    public void AddImage(string filePath)
    {
      var img = _currentSection.AddImage(filePath);
      img.RelativeHorizontal = RelativeHorizontal.Page;
      img.RelativeVertical = RelativeVertical.Page;

      img.Top = 0;
      img.Left = 0;

      img.Height = _currentDocument.DefaultPageSetup.PageHeight;
      img.Width = _currentDocument.DefaultPageSetup.PageWidth;

      _currentSection.AddPageBreak();

      Images.Add(filePath);
    }

    public void SaveDocument(string outDirectory)
    {
      if (_currentDocument != null && Images.Count > 0)
      {
        var render = new PdfDocumentRenderer();
        render.Document = _currentDocument;
        render.RenderDocument();
        render.Save(Path.Combine(outDirectory, $"images_{DateTime.Now:MM-dd-yy_H-mm-ss}.pdf"));
        _currentDocument = null;
      }
    }

    public void CreateNewDocument()
    {
      _currentDocument = new Document();
      _currentSection = _currentDocument.AddSection();
      Images = new List<string>();
    }
  }
}
