using System.Collections.Generic;
using System.Linq.Expressions;

namespace Task1
{
  public class ParameterToConstantTransform : ExpressionVisitor
  {
    Dictionary<string, object> _substitutions;

    public ParameterToConstantTransform(Dictionary<string, object> substitutions)
    {
      _substitutions = substitutions ?? new Dictionary<string, object>();
    }

    protected override Expression VisitLambda<T>(Expression<T> node)
    {
      var newBody = Visit(node.Body);
      return Expression.Lambda(newBody, node.Parameters);
    }

    protected override Expression VisitParameter(ParameterExpression node)
    {
      if (_substitutions.TryGetValue(node.Name, out object newValue))
      {
        return Expression.Constant(newValue, node.Type);
      }

      return base.VisitParameter(node);
    }
  }
}