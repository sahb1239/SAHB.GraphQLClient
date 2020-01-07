using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using SAHB.GraphQLClient.FieldBuilder;
using SAHB.GraphQLClient.Internal;

namespace SAHB.GraphQLClient.Filtering
{
    public class QueryGeneratorFilter : IQueryGeneratorFilter
    {
        public Func<GraphQLField, bool> GetFilter<T>(Expression<Func<T, T>> expression)
        {
            var memberNames = ExpressionHelper.GetMemberNamesFromExpression(expression);
            var queryGeneratorField = new QueryGeneratorField(memberNames);
            return queryGeneratorField.GetFunc();
        }

        private class QueryGeneratorField
        {
            private readonly IEnumerable<string> memberNames;

            public QueryGeneratorField(IEnumerable<string> memberNames)
            {
                this.memberNames = memberNames;
            }

            public Func<GraphQLField, bool> GetFunc()
            {
                return field => ValidateField(field);
            }

            public bool ValidateField(GraphQLField field)
            {
                return memberNames.Any(memberName =>
                    memberName.Equals(field.Path) ||
                    memberName.StartsWith(field.Path + "."));
            }
        }
    }
}
