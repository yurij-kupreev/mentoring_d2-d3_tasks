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

      this.ProceedImage(img);

      _currentSection.AddPageBreak();

      Images.Add(filePath);
    }

    public void AddImage(byte[] imageArr, bool isLast = false)
    {
      var fileNameBase64 = MigraDocFilenameFromByteArray(imageArr);

      var image = new Image(fileNameBase64);

      this.ProceedImage(image);

      if (isLast)
      {
        _currentSection.AddPageBreak();
      }

      Images.Add(fileNameBase64);
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

    public static string MigraDocFilenameFromByteArray(byte[] image)
    {
      return "base64:" +
             Convert.ToBase64String(image);
    }
  }
}
