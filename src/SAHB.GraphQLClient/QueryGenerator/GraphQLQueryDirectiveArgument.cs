using System;
using System.Linq.Expressions;
using SAHB.GraphQLClient.FieldBuilder.Attributes;

namespace SAHB.GraphQLClient.QueryGenerator
{
    public class GraphQLQueryDirectiveArgument : GraphQLQueryArgument
    {
        /// <summary>
        /// Initializes a GraphQL argument used to contain variable value and type of a argument which is added to a query
        /// </summary>
        /// <param name="variableName">The variable name which should be set used in the <see cref="GraphQLArgumentsAttribute"/></param>
        /// <param name="directiveName">The directiveName which should have applied the argument</param>
        /// <param name="argumentValue">The value which is inserted in the variables part of the GraphQL query</param>
        public GraphQLQueryDirectiveArgument(string variableName, string directiveName, object argumentValue)
            : base(variableName, directiveName, null, argumentValue)
        {
            if (directiveName == null)
                throw new ArgumentNullException(nameof(directiveName));
        }

        /// <summary>
        /// Initializes a GraphQL argument used to contain variable value and type of a argument which is added to a query
        /// </summary>
        /// <param name="variableName">The variable name which should be set used in the <see cref="GraphQLArgumentsAttribute"/></param>
        /// <param name="directiveName">The directiveName which should have applied the argument</param>
        /// <param name="argumentValue">The value which is inserted in the variables part of the GraphQL query</param>
        /// <param name="field">The GraphQL field which should have applied the argument/param>
        public GraphQLQueryDirectiveArgument(string variableName, string directiveName, object argumentValue, string field)
            : base(variableName, directiveName, field, argumentValue)
        {
            if (directiveName == null)
                throw new ArgumentNullException(nameof(directiveName));
        }
    }

    public class GraphQLQueryDirectiveArgument<T> : GraphQLQueryArgument<T>
    {
        /// <summary>
        /// Initializes a GraphQL argument used to contain variable value and type of a argument which is added to a query
        /// </summary>
        /// <param name="variableName">The variable name which should be set used in the <see cref="GraphQLArgumentsAttribute"/></param>
        /// <param name="directiveName">The directiveName which should have applied the argument</param>
        /// <param name="argumentValue">The value which is inserted in the variables part of the GraphQL query</param>
        public GraphQLQueryDirectiveArgument(string variableName, string directiveName, object argumentValue, Expression<Func<T, object>> expression)
            : base(variableName, directiveName, argumentValue, expression)
        {
            if (directiveName == null)
                throw new ArgumentNullException(nameof(directiveName));
        }
    }
}
