using SAHB.GraphQLClient.FieldBuilder;

namespace SAHB.GraphQLClient.Tests.Benchmarks.GeneralBenchmarks
{
    public class FieldBuilderBenchmarks : GeneralFieldBenchmark
    {
        readonly IGraphQLFieldBuilder _fieldBuilder = new GraphQLFieldBuilder();

        public override void RunBenchmark<T>()
        {
            _fieldBuilder.GetFields(typeof(T));
        }
    }
}
