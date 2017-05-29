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

    private readonly string _pdfTempDirectory;

    public List<string> Images { get; private set; }

    public PdfHelper(string pdfTempDirectory)
    {
      _pdfTempDirectory = pdfTempDirectory;
    }

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

    public string SaveDocument()
    {
      if (_currentDocument != null && Images.Count > 0)
      {
        var render = new PdfDocumentRenderer();
        render.Document = _currentDocument;
        render.RenderDocument();

        var pdfFilePath = Path.Combine(_pdfTempDirectory, $"images_{DateTime.Now:MM-dd-yy_H-mm-ss}.pdf");

        render.Save(pdfFilePath);
        _currentDocument = null;

        return pdfFilePath;
      }

      return string.Empty;
    }

    public void CreateNewDocument()
    {
      _currentDocument = new Document();
      _currentSection = _currentDocument.AddSection();
      Images = new List<string>();
    }
  }
}
