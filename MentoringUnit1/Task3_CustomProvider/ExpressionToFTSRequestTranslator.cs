using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Sample03
{
	public class ExpressionToFTSRequestTranslator : ExpressionVisitor
	{
		StringBuilder resultString;

		public string Translate(Expression exp)
		{
			resultString = new StringBuilder();
			Visit(exp);

			return resultString.ToString();
		}

		protected override Expression VisitMethodCall(MethodCallExpression node)
		{
      if (node.Method.DeclaringType == typeof(Queryable)
                && node.Method.Name == "Where")
      {
        var predicate = node.Arguments[1];
        Visit(predicate);

        return node;
      }

		  if (node.Method.DeclaringType == typeof(string))
		  {
		    var memberExpression = node.Object as MemberExpression;
		    if (memberExpression == null)
		    {
		      return base.VisitMethodCall(node);
		    }

		    var isContains = node.Method.Name == "Contains";
		    var matchBefore = node.Method.Name == "EndsWith" || isContains;
		    var matchAfter = node.Method.Name == "StartsWith" || isContains;
		    if (matchBefore || matchAfter)
		    {
		      Visit(memberExpression);
		      resultString.Append("(");
		      if (matchBefore) resultString.Append("*");
		      Visit(node.Arguments[0]);
		      if (matchAfter) resultString.Append("*");
		      resultString.Append(")");
		      return node;
		    }
		  }

		  return base.VisitMethodCall(node);
    }

		protected override Expression VisitBinary(BinaryExpression node)
		{
      switch (node.NodeType)
      {
        case ExpressionType.Equal:
          Expression memberAccess = null;
          Expression constantExpression = null;

          if (node.Left.NodeType == ExpressionType.MemberAccess)
          {
            memberAccess = node.Left;
            constantExpression = node.Right.NodeType == ExpressionType.Constant ? node.Right : null;
          }
          else if (node.Right.NodeType == ExpressionType.MemberAccess)
          {
            memberAccess = node.Right;
            constantExpression = node.Left.NodeType == ExpressionType.Constant ? node.Left : null;
          }

          if (memberAccess == null)
            throw new NotSupportedException(string.Format("Property operand was not provided.", node.NodeType));
          if (constantExpression == null)
            throw new NotSupportedException(string.Format("Constant operand was not provided.", node.NodeType));

          Visit(memberAccess);
          resultString.Append("(");
          Visit(constantExpression);
          resultString.Append(")");
          break;
        case ExpressionType.AndAlso:
          Visit(node.Left);
          resultString.Append(" AND ");
          Visit(node.Right);
          break;
        default:
          throw new NotSupportedException(string.Format("Operation {0} is not supported", node.NodeType));
      };

      return node;
		}

		protected override Expression VisitMember(MemberExpression node)
		{
			resultString.Append(node.Member.Name).Append(":");

			return base.VisitMember(node);
		}

		protected override Expression VisitConstant(ConstantExpression node)
		{
			resultString.Append(node.Value);

			return node;
		}
	}
}
