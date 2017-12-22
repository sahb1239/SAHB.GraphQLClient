using SAHB.GraphQLClient.FieldBuilder;
using SAHB.GraphQLClient.QueryBuilder;

namespace SAHB.GraphQLClient.Benchmarks.Tests
{
    public class QueryBuilderBenchmarks : GeneralFieldBenchmark
    {
        readonly IGraphQLQueryBuilder _queryBuilder = new GraphQLQueryBuilder(new GraphQLFieldBuilder());

        public override void RunBenchmark<T>()
        {
            _queryBuilder.GetQuery(typeof(T));
        }
    }
}
