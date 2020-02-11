using SAHB.GraphQLClient.FieldBuilder.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SAHB.GraphQLClient.FieldBuilder
{
    /// <summary>
    /// GraphQL directive used to contain metadata which can be used for generating a GraphQL query
    /// </summary>
    public class GraphQLFieldDirective
    {
        /// <summary>
        /// Initilizes a GraphQL field directive used to contain metadata which can be used for generating a GraphQL query
        /// </summary>
        /// <param name="directiveName">The directive name used in the GraphQL query</param>
        /// <param name="arguments">The arguments used for the directive</param>
        internal GraphQLFieldDirective(string directiveName, IEnumerable<GraphQLDirectiveArgumentAttribute> arguments)
            : this(directiveName, arguments.Select(e => new GraphQLFieldArguments(e)))
        {
        }

        /// <summary>
        /// Initilizes a GraphQL field directive used to contain metadata which can be used for generating a GraphQL query
        /// </summary>
        /// <param name="directiveName">The directive name used in the GraphQL query</param>
        /// <param name="arguments">The arguments used for the directive</param>
        public GraphQLFieldDirective(string directiveName, IEnumerable<GraphQLFieldArguments> arguments)
        {
            DirectiveName = directiveName ?? throw new ArgumentNullException(nameof(directiveName));
            Arguments = (arguments ?? Enumerable.Empty<GraphQLFieldArguments>()).ToList();
        }

        /// <summary>
        /// Name of the Directive
        /// </summary>
        public string DirectiveName { get; }

        /// <summary>
        /// Arguments for the current directive
        /// </summary>
        public ICollection<GraphQLFieldArguments> Arguments { get; }

        /// <inheritdoc />
        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();
            builder.AppendLine("Directive: " + DirectiveName);
            if (Arguments.Any())
            {
                builder.AppendLine($"Arguments: {IndentAndAddStart(String.Join(Environment.NewLine, Arguments))}");
            }
            return builder.ToString();
        }

        private string IndentAndAddStart(string text)
        {
            return (Environment.NewLine + text).Replace(Environment.NewLine, Environment.NewLine + "   ");
        }
    }
}
