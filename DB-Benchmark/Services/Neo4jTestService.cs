using DB_Benchmark.Models.Enums;
using Neo4j.Driver;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DB_Benchmark.Services
{
    public class Neo4jTestService : BaseDbTestService
    {
        public override DatabaseSystem System => DatabaseSystem.Neo4j;

        public override string GetExampleConnectionStringFormat()
        {
            return $"bolt://localhost:7687|neo4j|secret123\n\n" +
                $"Format: (bolt connection string)|(username)|(password)\n" +
                $"Be sure to add the pipes ('|') in between the strings, and maintain the order they are in.";
        }

        public override async Task<int> RunTest(object queriesObject)
        {
            var connStrings = ConnectionString.Split('|');
            string uri = connStrings[0], user = connStrings[1], pass = connStrings[2];

            var driver = GraphDatabase.Driver(uri, AuthTokens.Basic(user, pass));
            var queryStrings = (List<string>)queriesObject;

            var queries = new List<Task<IResultCursor>>();

            using (var session = driver.AsyncSession())
            {
                foreach (var queryString in queryStrings)
                {
                    var readTrans = session.ReadTransactionAsync(tx =>
                    {
                        var result = tx.RunAsync(queryString);
                        return result;
                    });
                    queries.Add(readTrans);
                }

                var resultsCursor = await base.RunTest<IResultCursor>(queries);
                var current = resultsCursor.Select(r => r.Current);
                var results = (IEnumerator<int>)resultsCursor.Select(r => r.Current.Values.Select(v => v.Value).FirstOrDefault()).FirstOrDefault();
                //var resultCount = results.AsParallel().Sum();
                //return resultCount;
                return 10;
            }
        }

        public override object SearchTermsToQueryTasks()
        {
            var queries = new List<string>();

            foreach (var searchTerm in SearchTerms)
            {
                queries.Add($"MATCH (m) WHERE m.title =~ '(?i).*{searchTerm}.*' RETURN count(m);");
            }            

            return queries;
        }
    }
}
