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
        /// The variable name which should be set used in the <see cref="GraphQLArgumentsAttribute"/>
        /// </summary>
        public string VariableName { get; set; }

        /// <summary>
        /// The value which is inserted in the variables part of the GraphQL query
        /// </summary>
        public object ArgumentValue { get; set; }

        /// <summary>
        /// The field which should have the argument set
        /// </summary>
        public string Field { get; set; }
    }

    public class GraphQLQueryArgument<T> : GraphQLQueryArgument
    {
        /// <summary>
        /// Initializes a GraphQL argument used to contain variable value and type of a argument which is added to a query
        /// </summary>
        /// <param name="variableName">The variable name which should be set used in the <see cref="GraphQLArgumentsAttribute"/></param>
        /// <param name="argumentValue">The value which is inserted in the variables part of the GraphQL query</param>
        public GraphQLQueryArgument(string variableName, object argumentValue, Expression<Func<T, object>> expression) : base(variableName, GetMemberName<T>(expression), argumentValue)
        {
        }

        private static string GetMemberName<T>(Expression<Func<T, object>> expression)
        {
            return GetMemberName(expression.Body);
        }

        private static string GetMemberName(Expression expression)
        {
            if (expression is MemberExpression)
            {
                // Reference type property or field
                var memberExpression = (MemberExpression)expression;
                if (memberExpression.Expression is ParameterExpression)
                {
                    return memberExpression.Member.Name;
                }
                else
                {
                    return string.Join(".", GetMemberName(memberExpression.Expression), memberExpression.Member.Name);
                }
            }

            if (expression is MethodCallExpression)
            {
                // Reference type method
                var methodCallExpression = (MethodCallExpression)expression;
                return methodCallExpression.Method.Name;
            }

            if (expression is UnaryExpression)
            {
                // Property, field of method returning value type
                var unaryExpression = (UnaryExpression)expression;
                return GetMemberName(unaryExpression);
            }

            if (expression is LambdaExpression)
            {
                // Property, field of method returning value type
                var lambdaExpression = (LambdaExpression)expression;
                return GetMemberName(lambdaExpression.Body);
            }

            throw new NotImplementedException(expression.GetType().Name + " not implemented");
        }
    }
}