using DB_Benchmark.Models.Enums;
using DB_Benchmark.Models.MongoDB;
using MongoDB.Driver;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DB_Benchmark.Services
{
    public class MongoTestService : BaseDbTestService
    {
        private const string databaseName = "What2Watch";
        private const string collectionName = "Movies";

        public MongoTestService()
        {
            system = DatabaseSystem.MongoDB;
        }

        public override string GetExampleConnectionStringFormat()
        {
            return "mongodb://127.0.0.1:27017?compressors=disabled&gssapiServiceName=mongodb&maxPoolSize=10001" +
                "\n\nBe sure to add the '&maxPoolSize=10001' to the end of your connection string to prevent errors.";
        }

        public override async Task RunTest(object queriesObject)
        {
            await base.RunTest<long>(queriesObject);
        }

        public override object SearchTermsToQueryTasks()
        {
            base.RunTestChecks();

            var database = GetDatabase();
            var moviesCollection = database.GetCollection<Movie>(collectionName);

            var queries = new List<Task<long>>();

            foreach (var searchTerm in SearchTerms)
            {
                var movieFilterDef = new FilterDefinitionBuilder<Movie>();
                var movieFilter = movieFilterDef.Where(x => x.Title == searchTerm);
                queries.Add(moviesCollection.CountDocumentsAsync(movieFilter));
            }

            return queries;
        }

        private IMongoDatabase GetDatabase()
        {
            var mongoClient = new MongoClient(connectionString);
            return mongoClient.GetDatabase(databaseName);
        }
    }
}
