using System.Collections.Generic;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Toolchains.CsProj;
using SAHB.GraphQLClient.FieldBuilder.Attributes;

namespace SAHB.GraphQLClient.Tests.Benchmarks.GeneralBenchmarks
{
    public class MultipleRuntimes : ManualConfig
    {
        public MultipleRuntimes()
        {
            Add(Job.Default.With(CsProjCoreToolchain.NetCoreApp20)); // .NET Core 2.0
        }
    }

    [Config(typeof(MultipleRuntimes))]
    public abstract class GeneralFieldBenchmark
    {
        public abstract void RunBenchmark<T>();

        public class SimpleQuery1Field
        {
            public string A { get; set; }
        }

        [Benchmark]
        public void TestSimpleQuery1Field()
        {
            RunBenchmark<SimpleQuery1Field>();
        }

        public class SimpleQuery2Field
        {
            public string A { get; set; }
            public string B { get; set; }
        }

        [Benchmark]
        public void TestSimpleQuery2Field()
        {
            RunBenchmark<SimpleQuery2Field>();
        }

        public class SimpleQuery3Field
        {
            public string A { get; set; }
            public string B { get; set; }
            public string C { get; set; }
        }

        [Benchmark]
        public void TestSimpleQuery3Field()
        {
            RunBenchmark<SimpleQuery3Field>();
        }

        public class SimpleQuery4Field
        {
            public string A { get; set; }
            public string B { get; set; }
            public string C { get; set; }
            public string D { get; set; }
        }

        [Benchmark]
        public void TestSimpleQuery4Field()
        {
            RunBenchmark<SimpleQuery4Field>();
        }

        public class SimpleQuery
        {
            public string A { get; set; }
            public string B { get; set; }
        }

        [Benchmark]
        public void TestSimpleFieldBuilder()
        {
            RunBenchmark<SimpleQuery>();
        }

        public class NestedQuery
        {
            public SimpleQuery A { get; set; }
            public SimpleQuery B { get; set; }
        }

        [Benchmark]
        public void TestNestedFieldBuilder()
        {
            RunBenchmark<NestedQuery>();
        }

        public class QueryWithFieldNames
        {
            public SimpleQuery A { get; set; }

            [GraphQLFieldName("other")]
            public SimpleQuery B { get; set; }
        }

        [Benchmark]
        public void TestQueryWithFieldNames()
        {
            RunBenchmark<QueryWithFieldNames>();
        }

        public class QueryWithFieldNamesAndIEnumerable
        {
            public SimpleQuery A { get; set; }

            [GraphQLFieldName("other")]
            public IEnumerable<SimpleQuery> B { get; set; }
        }

        [Benchmark]
        public void TestQueryWithFieldNamesAndIEnumerable()
        {
            RunBenchmark<QueryWithFieldNamesAndIEnumerable>();
        }
    }
}
