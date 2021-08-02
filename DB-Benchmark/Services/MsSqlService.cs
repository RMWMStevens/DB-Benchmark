﻿using DB_Benchmark.Models.Enums;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace DB_Benchmark.Services
{
    public class MsSqlService : BaseDbTestService
    {
        public MsSqlService()
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
            await base.RunTest(queriesObject);
            var queries = (List<string>)queriesObject;

            await RunTest(queries);
        }

        private async Task RunTest(List<string> queries) 
        {
            var runTasks = new List<Task>();

            foreach (var query in queries)
            {
                runTasks.Add(RunQueryAsync(query));
            }

            await Task.WhenAll();
        }

        public override object SearchTermsToQueries()
        {
            var queries = new List<string>();

            foreach (var searchTerm in searchTerms)
            {
                queries.Add($"SELECT Title FROM MOVIES WHERE Title LIKE '%{searchTerm}%'");
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
