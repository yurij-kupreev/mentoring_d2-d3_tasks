using System;
using System.Collections.Generic;
using System.IO;
using MigraDoc.DocumentObjectModel;
using MigraDoc.DocumentObjectModel.Shapes;
using MigraDoc.Rendering;
using Common.Models;

namespace Common.Helpers
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

    public CustomFile SaveDocument()
    {
      if (_currentDocument != null && Images.Count > 0)
      {
        var render = new PdfDocumentRenderer();
        render.Document = _currentDocument;
        render.RenderDocument();

        var pdfFileName = $"images_{DateTime.Now:MM-dd-yy_H-mm-ss}.pdf";

        var stream = new MemoryStream();
        render.Save(stream, false);

        var fileBytes = stream.ToArray();

        var file = new CustomFile(pdfFileName, fileBytes);

        _currentDocument = null;

        return file;
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
