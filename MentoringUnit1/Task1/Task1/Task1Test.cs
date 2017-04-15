using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Task1
{
  [TestClass]
  public class Task1Test
  {
    readonly TraceExpressionVisitor _tracer = new TraceExpressionVisitor();

    [TestMethod]
    public void CanReplaceAddOneWithIncrement()
    {
      Expression<Func<int, int>> expression = a => a + 1;
      var binaryToUnaryTransform = new IncrementDecrementTransform();
      var resultExpression = binaryToUnaryTransform.VisitAndConvert(expression, nameof(CanReplaceAddOneWithIncrement));

      DumpResult(expression, resultExpression);

      int expectedResult = expression.Compile().Invoke(1);
      int actualResult = resultExpression.Compile().Invoke(1);
      Assert.AreEqual(expectedResult, actualResult);
    }

    [TestMethod]
    public void CanReplaceOnlyAddOneWithIncrement()
    {
      Expression<Func<int, int>> expression = a => a + 2;
      var binaryToUnaryTransform = new IncrementDecrementTransform();
      var resultExpression = binaryToUnaryTransform.VisitAndConvert(expression, nameof(CanReplaceOnlyAddOneWithIncrement));

      DumpResult(expression, resultExpression);

      int expectedResult = expression.Compile().Invoke(1);
      int actualResult = resultExpression.Compile().Invoke(1);
      Assert.AreEqual(expectedResult, actualResult);
    }

    [TestMethod]
    public void CanReplaceSubstractOneWithIncrement()
    {
      Expression<Func<int, int>> expression = a => a - 1;
      var binaryToUnaryTransform = new IncrementDecrementTransform();
      var resultExpression = binaryToUnaryTransform.VisitAndConvert(expression, nameof(CanReplaceSubstractOneWithIncrement));

      DumpResult(expression, resultExpression);

      int expectedResult = expression.Compile().Invoke(1);
      int actualResult = resultExpression.Compile().Invoke(1);
      Assert.AreEqual(expectedResult, actualResult);
    }

    [TestMethod]
    public void CanReplaceParameterCallsWithConstants()
    {
      const int aValue = 1, bValue = 3;
      Expression<Func<int, int, int>> expression = (a, b) => a + b;
      var substitutions = new Dictionary<string, object> { { "a", aValue } };
      var paramToConstTransform = new ParameterToConstantTransform(substitutions);
      var resultExpression = paramToConstTransform.VisitAndConvert(expression, nameof(CanReplaceParameterCallsWithConstants));

      DumpResult(expression, resultExpression);

      int expectedResult = expression.Compile().Invoke(aValue, bValue);
      int actualResult = resultExpression.Compile().Invoke(aValue + 3, bValue);
      Assert.AreEqual(expectedResult, actualResult);
    }

    [TestMethod]
    public void AddToIncrementTransformTest()
    {
      Expression<Func<int, int>> sourceExp = (a) => a + (a + 1) * (a + 5) * (a - 1);
      var newExp = (new IncrementDecrementTransform().VisitAndConvert(sourceExp, ""));

      DumpResult(sourceExp, newExp);

      var sourceExpResult = sourceExp.Compile().Invoke(3);
      var newExpResult = newExp.Compile().Invoke(3);

      Console.WriteLine(sourceExp + ": " + sourceExpResult);
      Console.WriteLine(newExp + ": " + newExpResult);

      Assert.AreEqual(sourceExpResult, newExpResult);

    }

    private void DumpResult(Expression source, Expression result)
    {
      Console.WriteLine("Source expression:");
      _tracer.Visit(source);
      Console.WriteLine("Transformed expression:");
      _tracer.Visit(result);
    }
  }
}
