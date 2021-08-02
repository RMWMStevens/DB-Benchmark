using DB_Benchmark.Models.Enums;
using MongoDB.Driver;
using System.Threading.Tasks;

namespace DB_Benchmark.Services
{
    public class MongoService : BaseDbTestService
    {
        private const string databaseName = "What2Watch";

        public MongoService()
        {
            system = DatabaseSystem.MongoDB;
        }

        public override string GetExampleConnectionStringFormat()
        {
            return @"mongodb://127.0.0.1:27017/?compressors=disabled&gssapiServiceName=mongodb";
        }

        public override async Task RunTest(object queriesObject)
        {
            await base.RunTest(queriesObject);
        }

        public override object SearchTermsToQueries()
        {
            throw new System.NotImplementedException();
        }

        private IMongoDatabase GetDatabase()
        {
            var mongoClient = new MongoClient(connectionString);
            return mongoClient.GetDatabase(databaseName);
        }
    }
}
