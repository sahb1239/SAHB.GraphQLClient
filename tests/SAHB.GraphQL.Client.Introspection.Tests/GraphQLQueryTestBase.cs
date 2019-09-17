using GraphQL.Types;
using SAHB.GraphQL.Client.Testserver.Schemas;
using SAHB.GraphQL.Client.TestServer;
using SAHB.GraphQLClient;
using System.Net.Http;
using Xunit;

namespace SAHB.GraphQL.Client.Introspection.Tests
{
    public abstract class GraphQLQueryTestBase<T> : GraphQLTestBase<GenericQuerySchema<T>>
        where T : ObjectGraphType, new()
    {
        public GraphQLQueryTestBase(GraphQLWebApplicationFactory<GenericQuerySchema<T>> factory) : base(factory)
        {
        }
    }

    public abstract class GraphQLTestBase<TSchema> : IClassFixture<GraphQLWebApplicationFactory<TSchema>>
        where TSchema : Schema
    {
        private readonly GraphQLWebApplicationFactory<TSchema> _factory;
        protected HttpClient Client => _factory.CreateClient();
        protected GraphQLHttpClient GraphQLHttpClient => GraphQLHttpClient.Default(Client);

        public GraphQLTestBase(GraphQLWebApplicationFactory<TSchema> factory)
        {
            _factory = factory;
        }
    }
}
