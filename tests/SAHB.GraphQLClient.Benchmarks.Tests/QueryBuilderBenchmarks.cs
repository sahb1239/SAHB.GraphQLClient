using BenchmarkDotNet.Attributes;
using SAHB.GraphQLClient.FieldBuilder;
using SAHB.GraphQLClient.QueryBuilder;

namespace SAHB.GraphQLClient.Benchmarks.Tests
{
    public class QueryBuilderBenchmarks
    {
        readonly IGraphQLQueryBuilder _queryBuilder = new GraphQLQueryBuilder(new GraphQLFieldBuilder());

        public class SimpleQuery
        {
            public string A { get; set; }
            public string B { get; set; }
        }

        [Benchmark]
        public void TestSimpleFieldBuilder()
        {
            _queryBuilder.GetQuery(typeof(FieldBuilderBenchmarks.SimpleQuery));
        }

        public class NestedQuery
        {
            public FieldBuilderBenchmarks.SimpleQuery A { get; set; }
            public FieldBuilderBenchmarks.SimpleQuery B { get; set; }
        }

        [Benchmark]
        public void TestNestedFieldBuilder()
        {
            _queryBuilder.GetQuery(typeof(FieldBuilderBenchmarks.NestedQuery));
        }
    }
}
