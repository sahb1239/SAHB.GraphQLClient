using System.Reflection;
using BenchmarkDotNet.Running;
using SAHB.GraphQLClient.Tests.Benchmarks;

namespace SAHB.GraphQLClient.Benchmarks
{
    class Program
    {
        static void Main(string[] args)
        {
            new BenchmarkSwitcher(typeof(Runner).GetTypeInfo().Assembly).RunAll();
        }
    }
}
