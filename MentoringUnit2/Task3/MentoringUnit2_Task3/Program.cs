using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Xsl;

namespace MentoringUnit2_Task3
{
  class Program
  {
    static int Main(string[] args)
    {
      if (args.Length != 3 || !File.Exists(args[0]) || !File.Exists(args[1]))
      {
        Console.WriteLine("Invalid arguments. Should be: 'xml file path' 'xslt file path' 'result html file path'", nameof(args));
        return 1;
      }

      var inputFilePath = args[0];
      var xsltFilePath = args[1];
      var resultFilePath = args[2];

      var transform = new XslCompiledTransform();
      transform.Load(xsltFilePath, new XsltSettings { EnableScript = true }, null);
      transform.Transform(inputFilePath, resultFilePath);

      Console.WriteLine("Success");
      return 0;
    }
  }
}
