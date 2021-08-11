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

        public override async Task RunTest(object queriesObject)
        {
            var connStrings = ConnectionString.Split('|');
            string uri = connStrings[0], user = connStrings[1], pass = connStrings[2];

            var driver = GraphDatabase.Driver(uri, AuthTokens.Basic(user, pass));
            var queryStrings = (List<string>)queriesObject;

            using (var session = driver.AsyncSession())
            {
                var resultSum = await session.ReadTransactionAsync(async tx =>
                {
                    var queries = new List<Task<IResultCursor>>();

                    foreach (var queryString in queryStrings)
                    {
                        queries.Add(tx.RunAsync(queryString));
                    }

                    var resultCursors = await base.RunTest<IResultCursor>(queries);

                    var fetchTasks = new List<Task<bool>>();

                    foreach (var resultCursor in resultCursors)
                    {
                        fetchTasks.Add(resultCursor.FetchAsync());
                    }

                    var fetchResults = await Task.WhenAll(fetchTasks);

                    var currents = resultCursors.Select(r => r.Current);
                    var resultCounts = (resultCursors.Select(r => r.Current.Values.Select(v => (long)v.Value).FirstOrDefault()));
                    var resultSum = resultCounts.AsParallel().Sum();
                    return resultSum;
                });
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
