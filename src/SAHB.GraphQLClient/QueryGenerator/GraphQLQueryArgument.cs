using System;
using System.Linq.Expressions;
using SAHB.GraphQLClient.FieldBuilder.Attributes;

namespace SAHB.GraphQLClient.QueryGenerator
{
    // ReSharper disable once InconsistentNaming
    /// <summary>
    /// GraphQL argument used to contain variable value and type of a argument which is added to a query
    /// </summary>
    public class GraphQLQueryArgument
    {
        /// <summary>
        /// Initializes a GraphQL argument used to contain variable value and type of a argument which is added to a query
        /// </summary>
        /// <param name="variableName">The variable name which should be set used in the <see cref="GraphQLArgumentsAttribute"/></param>
        /// <param name="argumentValue">The value which is inserted in the variables part of the GraphQL query</param>
        public GraphQLQueryArgument(string variableName, object argumentValue)
        {
            VariableName = variableName ?? throw new ArgumentNullException(nameof(variableName));
            ArgumentValue = argumentValue ?? throw new ArgumentNullException(nameof(argumentValue));
        }

        /// <summary>
        /// Initializes a GraphQL argument used to contain variable value and type of a argument which is added to a query
        /// </summary>
        /// <param name="variableName">The variable name which should be set used in the <see cref="GraphQLArgumentsAttribute"/></param>
        /// <param name="field">The GraphQL field which should have applied the argument/param>
        /// <param name="argumentValue">The value which is inserted in the variables part of the GraphQL query</param>
        public GraphQLQueryArgument(string variableName, string field, object argumentValue)
            : this(variableName, argumentValue)
        {
            Field = field;
        }

        /// <summary>
        /// Initializes a GraphQL argument used to contain variable value and type of a argument which is added to a query
        /// </summary>
        /// <param name="variableName">The variable name which should be set used in the <see cref="GraphQLArgumentsAttribute"/></param>
        /// <param name="directiveName">The directiveName which should have applied the argument</param>
        /// <param name="field">The GraphQL field which should have applied the argument/param>
        /// <param name="argumentValue">The value which is inserted in the variables part of the GraphQL query</param>
        public GraphQLQueryArgument(string variableName, string directiveName, string field, object argumentValue)
            : this(variableName, field, argumentValue)
        {
            DirectiveName = directiveName;
        }

        /// <summary>
        /// The variable name which should be set used in the <see cref="GraphQLArgumentsAttribute"/>
        /// </summary>
        public string VariableName { get; set; }

        /// <summary>
        /// The directive name which the argument should be applied to
        /// </summary>
        public string DirectiveName { get; set; }

        /// <summary>
        /// The value which is inserted in the variables part of the GraphQL query
        /// </summary>
        public object ArgumentValue { get; set; }

        /// <summary>
        /// The field which should have the argument set
        /// </summary>
        public string Field { get; set; }
    }

    // ReSharper disable once InconsistentNaming
    /// <summary>
    /// GraphQL argument used to contain variable value and type of a argument and a expression of which field which the argument should be applied to
    /// </summary>
    public class GraphQLQueryArgument<T> : GraphQLQueryArgument
    {
        /// <summary>
        /// Initializes a GraphQL argument used to contain variable value and type of a argument which is added to a query
        /// </summary>
        /// <param name="variableName">The variable name which should be set used in the <see cref="GraphQLArgumentsAttribute"/></param>
        /// <param name="argumentValue">The value which is inserted in the variables part of the GraphQL query</param>
        public GraphQLQueryArgument(string variableName, object argumentValue, Expression<Func<T, object>> expression)
            : base(variableName, GetMemberName(expression), argumentValue)
        {
        }

        /// <summary>
        /// Initializes a GraphQL argument used to contain variable value and type of a argument which is added to a query
        /// </summary>
        /// <param name="variableName">The variable name which should be set used in the <see cref="GraphQLArgumentsAttribute"/></param>
        /// <param name="directiveName">The directiveName which should have applied the argument</param>
        /// <param name="argumentValue">The value which is inserted in the variables part of the GraphQL query</param>
        public GraphQLQueryArgument(string variableName, string directiveName, object argumentValue, Expression<Func<T, object>> expression)
            : base(variableName, directiveName, GetMemberName(expression), argumentValue)
        {
        }

        private static string GetMemberName(Expression<Func<T, object>> expression)
        {
            return GetMemberName(expression.Body);
        }

        private static string GetMemberName(Expression expression)
        {
            if (expression is MemberExpression memberExpression)
            {
                // Reference type property or field
                if (memberExpression.Expression is ParameterExpression)
                {
                    return memberExpression.Member.Name;
                }
                else
                {
                    return string.Join(".", GetMemberName(memberExpression.Expression), memberExpression.Member.Name);
                }
            }

            if (expression is MethodCallExpression methodCallExpression)
            {
                // Reference type method
                return methodCallExpression.Method.Name;
            }

            if (expression is UnaryExpression unaryExpression)
            {
                // Property, field of method returning value type
                return GetMemberName(unaryExpression);
            }

            if (expression is LambdaExpression lambdaExpression)
            {
                // Property, field of method returning value type
                return GetMemberName(lambdaExpression.Body);
            }

            throw new NotImplementedException(expression.GetType().Name + " not implemented");
        }
    }
}
