using System.Linq.Expressions;

namespace Task1
{
  public class IncrementDecrementTransform : ExpressionVisitor
  {
    protected override Expression VisitBinary(BinaryExpression node)
    {
      if (node.NodeType == ExpressionType.Add || node.NodeType == ExpressionType.Subtract)
      {
        ParameterExpression param = null;
        ConstantExpression constant = null;
        switch (node.Left.NodeType)
        {
          case ExpressionType.Parameter:
            param = (ParameterExpression)node.Left;
            break;
          case ExpressionType.Constant:
            constant = (ConstantExpression)node.Left;
            break;
        }

        switch (node.Right.NodeType)
        {
          case ExpressionType.Parameter:
            param = (ParameterExpression)node.Right;
            break;
          case ExpressionType.Constant:
            constant = (ConstantExpression)node.Right;
            break;
        }

        if (param != null && constant != null && constant.Type == typeof(int) && (int)constant.Value == 1)
        {
          return node.NodeType == ExpressionType.Add ? Expression.Increment(param) : Expression.Decrement(param);
        }

      }

      return base.VisitBinary(node);
    }
  }
}