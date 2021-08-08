using DB_Benchmark.Models.Enums;
using System;
using System.Threading.Tasks;

namespace DB_Benchmark.Services
{
    public class RedisTestService : BaseDbTestService
    {
        public override DatabaseSystem System => DatabaseSystem.Redis;

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
