using DB_Benchmark.Models.Enums;
using System.Threading.Tasks;

namespace DB_Benchmark.Services
{
    public class MongoService : BaseDbTestService
    {
        public MongoService()
        {
            system = DatabaseSystem.MongoDB;
        }

        public override string GetExampleConnectionStringFormat()
        {
            return @"mongodb://127.0.0.1:27017/?compressors=disabled&gssapiServiceName=mongodb";
        }

        public override async Task RunTest()
        {
            await base.RunTest();
        }
    }
}
