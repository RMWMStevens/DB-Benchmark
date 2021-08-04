using DB_Benchmark.Models.Enums;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace DB_Benchmark.Services
{
    public class MsSqlTestService : BaseDbTestService
    {
        public MsSqlTestService()
        {
            system = DatabaseSystem.MSSQL;
        }

        public override string GetExampleConnectionStringFormat()
        {
            var sqlAuth = @"Data Source=COMPUTERNAME;Initial Catalog=DATABASENAME;User ID=USERNAME;Password=PASSWORD;Connection Timeout=600";
            var winAuth = @"Data Source=COMPUTERNAME;Initial Catalog=DATABASENAME;Integrated Security=SSPI;Connection Timeout=600";

            return $"For SQL Authentication: \n{sqlAuth}\nFor Windows Authentication (leave 'SSPI' as is): \n{winAuth}" +
                $"\n\nThe 'Connection Timeout=(value) part makes sure the connection doesn't close prematurely.";
        }

        public override async Task RunTest(object queriesObject)
        {
            await base.RunTest<int>(queriesObject);
        }

        public override object SearchTermsToQueryTasks()
        {
            base.RunTestChecks();

            var queries = new List<Task<int>>();

            foreach (var searchTerm in SearchTerms)
            {
                var query = $"SELECT Title FROM MOVIES WHERE Title LIKE '%{searchTerm}%'";
                queries.Add(RunQueryAsync(query));
            }

            return queries;
        }

        public async Task<int> RunQueryAsync(string sqlQuery)
        {
            var sqlConnection = new SqlConnection(connectionString);
            var command = new SqlCommand(sqlQuery, sqlConnection);

            await sqlConnection.OpenAsync();
            var resultCount = await command.ExecuteNonQueryAsync();
            await sqlConnection.CloseAsync();

            return resultCount;
        }
    }
}
