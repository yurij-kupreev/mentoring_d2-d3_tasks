using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Xsl;

namespace MentoringUnit2_Task2
{
  public class Program
  {
    private const string XsltFileName = "books_rss_parser.xslt";

    public static int Main(string[] args)
    {
      if (args.Length == 0)
      {
        Console.WriteLine("Enter file name.");
        return 1;
      }

      var filePath = args[0];

      var file = new FileInfo(filePath);
      var newFileName = file.Name + "_rss.xml";

      var transform = new XslCompiledTransform();
      transform.Load(XsltFileName);

      transform.Transform(filePath, newFileName);

      Console.WriteLine("Transfor completed");

      return 0;
    }
  }
}
