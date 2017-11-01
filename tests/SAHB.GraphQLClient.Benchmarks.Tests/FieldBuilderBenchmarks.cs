using BenchmarkDotNet.Attributes;
using SAHB.GraphQLClient.FieldBuilder;

namespace SAHB.GraphQLClient.Benchmarks.Tests
{
    public class FieldBuilderBenchmarks
    {
        readonly IGraphQLFieldBuilder _fieldBuilder = new GraphQLFieldBuilder();

        public class SimpleQuery
        {
            public string A { get; set; }
            public string B { get; set; }
        }

        [Benchmark]
        public void TestSimpleFieldBuilder()
        {
            _fieldBuilder.GetFields(typeof(SimpleQuery));
        }

        public class NestedQuery
        {
            public SimpleQuery A { get; set; }
            public SimpleQuery B { get; set; }
        }

        [Benchmark]
        public void TestNestedFieldBuilder()
        {
            _fieldBuilder.GetFields(typeof(NestedQuery));
        }
    }
}
