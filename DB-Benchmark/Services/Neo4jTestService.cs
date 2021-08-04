using System;
using System.Threading.Tasks;

namespace DB_Benchmark.Services
{
    public class Neo4jTestService : BaseDbTestService
    {
        public override string GetExampleConnectionStringFormat()
        {
            throw new NotImplementedException();
        }

        public override Task RunTest(object queriesObject)
        {
            throw new NotImplementedException();
        }

        public override object SearchTermsToQueryTasks()
        {
            throw new NotImplementedException();
        }
    }
}
