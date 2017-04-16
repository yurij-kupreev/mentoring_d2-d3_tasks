using System;
using XmlValidator;

namespace MentoringUnit2_Task1
{
  public class CatalogXmlValidatorUtility
  {
    private const string NameSpace = "http://library.by/catalog";
    private const string CsdFileName = "books.xsd";

    private static readonly XmlValidatorService XmlValidator = new XmlValidatorService(NameSpace, CsdFileName);

    public static int Main(string[] args)
    {
      if (args.Length == 0)
      {
        Console.WriteLine("Enter file name.");
        return 1;
      }

      var filePath = args[0];

      var result = XmlValidator.Validate(filePath);

      Console.WriteLine(result ? "Xml file valid." : "Xml file not valid.");

      return 0;
    }
  }
}
