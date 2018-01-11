using SAHB.GraphQLClient.FieldBuilder;
using SAHB.GraphQLClient.QueryGenerator;
using SAHB.GraphQLClient.Extentions;

namespace SAHB.GraphQLClient.Tests.Benchmarks.GeneralBenchmarks
{
    public class QueryGeneratorBenchmarks : GeneralFieldBenchmark
    {
        readonly IGraphQLQueryGeneratorFromFields _queryGenerator = new GraphQLQueryGeneratorFromFields();
        readonly IGraphQLFieldBuilder _fieldBuilder = new GraphQLFieldBuilder();

        public override void RunBenchmark<T>()
        {
            _queryGenerator.GetQuery<T>(_fieldBuilder);
        }
    }
}
