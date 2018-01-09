using SAHB.GraphQLClient.FieldBuilder;
using SAHB.GraphQLClient.QueryGenerator;

namespace SAHB.GraphQLClient.Tests.Benchmarks.GeneralBenchmarks
{
    public class QueryGeneratorBenchmarks : GeneralFieldBenchmark
    {
        readonly IGraphQLQueryGenerator _queryGenerator = new GraphQLQueryGenerator(new GraphQLFieldBuilder());

        public override void RunBenchmark<T>()
        {
            _queryGenerator.GetQuery(typeof(T));
        }
    }
}
