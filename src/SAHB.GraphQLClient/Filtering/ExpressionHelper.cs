using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace SAHB.GraphQLClient.Filtering
{
    internal class ExpressionHelper
    {
        internal static IEnumerable<string> GetMemberNamesFromExpression(Expression expression)
        {
            if (expression is MemberExpression memberExpression)
            {
                return new[] { GetMemberNameFromMemberExpression(memberExpression) };
            }
            else if (expression is NewExpression newExpression)
            {
                return GetMemberNamesFromNewExpression(newExpression);
            }
            else if (expression is MemberInitExpression memberInitExpression)
            {
                return GetMemberNamesFromMemberInitExpression(memberInitExpression);
            }
            else if (expression is LambdaExpression lambdaExpression)
            {
                return GetMemberNamesFromLambdaExpression(lambdaExpression);
            }
            else
            {
                throw new NotImplementedException(expression.ToString());
            }
        }

        internal static IEnumerable<string> GetMemberNamesFromNewExpression(NewExpression newExpression)
        {
            for (int i = 0; i < newExpression.Arguments.Count; i++)
            {
                var argument = newExpression.Arguments[i];
                var member = newExpression.Members[i];

                foreach (var memberName in GetMemberNamesFromExpression(argument))
                {
                    yield return memberName;
                }
            }
        }

        private static IEnumerable<string> GetMemberNamesFromMemberInitExpression(MemberInitExpression memberInitExpression)
        {
            var memberNamesFromNewExpression = GetMemberNamesFromNewExpression(memberInitExpression.NewExpression);
            return memberNamesFromNewExpression.Concat(memberInitExpression.Bindings.SelectMany(GetMemberNamesFromMemberBinding)).ToList();
        }

        private static IEnumerable<string> GetMemberNamesFromMemberBinding(MemberBinding memberBinding)
        {
            if (memberBinding is MemberAssignment memberAssignment)
            {
                return GetMemberNamesFromExpression(memberAssignment.Expression);
            }
            else
            {
                throw new NotImplementedException($"{memberBinding.BindingType} not implemented");
            }
        }

        private static IEnumerable<string> GetMemberNamesFromLambdaExpression(LambdaExpression expression)
        {
            return GetMemberNamesFromExpression(expression.Body);
        }

        private static string GetMemberName(Expression expression)
        {
            if (expression is MemberExpression memberExpression)
            {
                return GetMemberNameFromMemberExpression(memberExpression);
            }
            else if (expression is UnaryExpression unaryExpression)
            {
                return GetMemberNameFromUnaryExpression(unaryExpression);
            }
            else if (expression is LambdaExpression lambdaExpression)
            {
                return GetMemberNameFromLambdaExpression(lambdaExpression);
            }
            else
            {
                throw new NotImplementedException(expression.ToString());
            }
        }

        private static string GetMemberNameFromMemberExpression(MemberExpression expression)
        {
            // Reference type property or field
            if (expression.Expression is ParameterExpression)
            {
                return expression.Member.Name;
            }
            else
            {
                return string.Join(".", GetMemberName(expression.Expression), expression.Member.Name);
            }
        }

        private static string GetMemberNameFromUnaryExpression(UnaryExpression expression)
        {
            // Property, field of method returning value type
            return GetMemberName(expression.Operand);
        }

        private static string GetMemberNameFromLambdaExpression(LambdaExpression expression)
        {
            // Property, field of method returning value type
            return GetMemberName(expression.Body);
        }
    }
}
