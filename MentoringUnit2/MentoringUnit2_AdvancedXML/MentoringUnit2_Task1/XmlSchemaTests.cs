using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MentoringUnit2_Task1
{
  [TestClass]
  public class BookXmlSchemaTests
  {
    private const string NameSpace = "http://library.by/catalog";
    private const string CsdFileName = "books.xsd";

    private readonly XmlValidator _xmlValidator;

    public BookXmlSchemaTests()
    {
      _xmlValidator = new XmlValidator(NameSpace, CsdFileName);
    }

    [TestMethod]
    public void CanValidateCorrectCatalog()
    {
      var docPath = "books.xml";
      var result = _xmlValidator.Validate(docPath);
      Assert.IsTrue(result);
    }

    [TestMethod]
    public void CanValidateIncorrectCatalog()
    {
      var docPath = "books_invalid.xml";
      var result = _xmlValidator.Validate(docPath);
      Assert.IsFalse(result);
    }
  }
}
