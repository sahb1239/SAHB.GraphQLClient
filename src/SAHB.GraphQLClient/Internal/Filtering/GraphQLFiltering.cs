using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Newtonsoft.Json;

namespace SAHB.GraphQLClient.Internal.Filtering
{
    internal class GraphQLFiltering : IGraphQLFiltering
    {
        public string GetFilterArgument(Expression expression)
        {
            return JsonConvert.SerializeObject(GetFilterExpressionObj(expression));
        }

        object GetFilterExpressionObj(Expression expression)
        {
            if (expression is LambdaExpression lambdaExpression)
            {
                return GetFilterExpressionObj(lambdaExpression.Body);
            }
            else if (expression is BinaryExpression binaryExpression)
            {
                var left = GetFilterExpressionObj(binaryExpression.Left);
                var right = GetFilterExpressionObj(binaryExpression.Right);
                string type;

                switch (binaryExpression.NodeType)
                {
                    case ExpressionType.Equal:
                        type = "==";
                        break;
                    case ExpressionType.AndAlso:
                        type = "&&";
                        break;
                    case ExpressionType.OrElse:
                        type = "||";
                        break;
                    case ExpressionType.GreaterThan:
                        type = ">";
                        break;
                    case ExpressionType.GreaterThanOrEqual:
                        type = ">=";
                        break;
                    case ExpressionType.LessThan:
                        type = "<";
                        break;
                    case ExpressionType.LessThanOrEqual:
                        type = "<=";
                        break;
                    case ExpressionType.Add:
                        type = "+";
                        break;
                    case ExpressionType.Subtract:
                    case ExpressionType.SubtractChecked:
                        type = "-";
                        break;
                    case ExpressionType.Multiply:
                        type = "*";
                        break;
                    case ExpressionType.Divide:
                        type = "/";
                        break;
                    default:
                        throw new NotSupportedException();
                }

                return new Dictionary<string, List<object>>() { { type, new List<object> { left, right } } };
            }
            else if (expression is ParameterExpression parameterExpression)
            {
                return "self";
            }
            else if (expression is ConstantExpression constantExpression)
            {
                return constantExpression.Value;
            }
            else if (expression is MemberExpression memberExpression)
            {
                return string.Join(".", GetFilterExpressionObj(memberExpression.Expression), memberExpression.Member.Name);
            }
            else
            {
                throw new NotSupportedException();
            }
        }
    }
}