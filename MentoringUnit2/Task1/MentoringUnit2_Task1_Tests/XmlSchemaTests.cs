using Microsoft.VisualStudio.TestTools.UnitTesting;
using XmlValidator;

namespace MentoringUnit2_Task1_Tests
{
  [TestClass]
  public class BookXmlSchemaTests
  {
    private const string NameSpace = "http://library.by/catalog";
    private const string CsdFileName = "books.xsd";

    private readonly XmlValidatorService _xmlValidator;

    public BookXmlSchemaTests()
    {
      _xmlValidator = new XmlValidatorService(NameSpace, CsdFileName);
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
