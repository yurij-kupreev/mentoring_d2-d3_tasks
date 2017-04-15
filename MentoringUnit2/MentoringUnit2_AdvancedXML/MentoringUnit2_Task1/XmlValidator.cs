using System;
using System.Xml;
using System.Xml.Schema;

namespace MentoringUnit2_Task1
{
  public class XmlValidator
  {
    private readonly string _nameSpace;
    private readonly string _xsdFileName;

    private static bool _succesResult;

    public XmlValidator(string nameSpace, string xsdFileName)
    {
      _nameSpace = nameSpace;
      _xsdFileName = xsdFileName;
    }

    public bool Validate(string docUrl)
    {
      _succesResult = true;

      var settings = new XmlReaderSettings();
      settings.Schemas.Add(_nameSpace, _xsdFileName);
      settings.ValidationType = ValidationType.Schema;
      settings.ValidationEventHandler += ErrorHandler;

      var reader = XmlReader.Create(docUrl, settings);
      var doc = new XmlDocument();

      try
      {
        doc.Load(reader);
      }
      catch (XmlSchemaValidationException)
      {
        return false;
      }

      return _succesResult;
    }

    private static void ErrorHandler(object sender, ValidationEventArgs e)
    {
      if (e.Severity == XmlSeverityType.Error)
      {
        _succesResult = false;
        Console.WriteLine(e.Message);
      }
    }
  }
}