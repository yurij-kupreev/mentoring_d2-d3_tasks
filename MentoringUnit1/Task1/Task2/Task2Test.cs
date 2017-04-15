using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Task1
{
  [TestClass]
  public class Task2Test
  {

    [TestMethod]
    public void TestMethod3()
    {
      var mapGenerator = new MappingGenerator();
      var mapper = mapGenerator.Generate<Foo, Bar>();

      var source = new Foo
      {
        A = 1,
        B = 2,
        C = "qwerty"
      };

      var res = mapper.Map(source);

      Console.WriteLine($"{res.A} {res.B} {res.C}");

      Assert.AreEqual(source.A, res.A);
      Assert.AreEqual(source.B, res.B);
      Assert.AreEqual(source.C, res.C);
    }
  }

  public class Foo
  {
    public int A { get; set; }

    public int B { get; set; }

    public string C { get; set; }
  }

  public class Bar
  {
    public int A { get; set; }

    public int B { get; set; }

    public string C { get; set; }
  }
}