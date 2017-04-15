using System;
using System.Linq.Expressions;

namespace Task1
{
  public class TraceExpressionVisitor : ExpressionVisitor
  {
    public int indent = 0;

    public override Expression Visit(Expression node)
    {
      if (node == null)
        return base.Visit(node);

      Console.WriteLine("{0}{1} - {2}", new string(' ', indent * 4),
          node.NodeType, node.GetType());

      indent++;
      Expression result = base.Visit(node);
      indent--;

      return result;
    }
  }
}