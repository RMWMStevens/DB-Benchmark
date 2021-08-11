using DB_Benchmark.Helpers;
using DB_Benchmark.Models.Enums;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DB_Benchmark.Services
{
    public class RedisTestService : BaseDbTestService
    {
        public override DatabaseSystem System => DatabaseSystem.Redis;

        public override string GetExampleConnectionStringFormat() => "localhost:6379";

        public override async Task LoadTest(TestProfile testProfile)
        {
            await base.LoadTest(testProfile);
            await CacheQueries();
        }

        public override async Task RunTest(object queriesObject)
        {
            var results = await base.RunTest<RedisValue>(queriesObject);
            return;
        }

        public override object SearchTermsToQueryTasks()
        {
            var db = GetDatabase();

            var queries = new List<Task<RedisValue>>();

            foreach (var searchTerm in SearchTerms)
            {
                queries.Add(db.StringGetAsync(searchTerm));
            }

            return queries;
        }

        private async Task CacheQueries()
        {
            var msSqlTestService = new MsSqlTestService();

            LogHelper.Disable();
            await msSqlTestService.LoadConfigFromFileSystemAsync();
            LogHelper.Enable();

            if (string.IsNullOrEmpty(msSqlTestService.ConnectionString)) { throw new ArgumentNullException("MsSqlTestService.ConnectionString", "MSSQL connection string not set. Set it first before trying to run Redis again"); }

            var db = GetDatabase();
            var searchTerms = new List<string>();
            var queryTasks = new List<Task<int>>();

            foreach (var searchTerm in SearchTerms)
            {
                if (await db.KeyExistsAsync(searchTerm)) { continue; }

                var query = $"SELECT Title FROM MOVIES WHERE Title LIKE '%{searchTerm}%'";
                searchTerms.Add(searchTerm);
                queryTasks.Add(msSqlTestService.RunQueryAsync(query));
            }

            var queryResults = await Task.WhenAll(queryTasks);

            var cacheSets = new List<KeyValuePair<RedisKey, RedisValue>>();

            for (int i = 0; i < searchTerms.Count; i++)
            {
                var searchTerm = searchTerms[i];
                var queryResult = queryResults[i];
                cacheSets.Add(new KeyValuePair<RedisKey, RedisValue>(searchTerm, queryResult));
            }

            await db.StringSetAsync(cacheSets.ToArray());
        }

        private IDatabase GetDatabase()
        {
            var redis = ConnectionMultiplexer.Connect(
                new ConfigurationOptions
                {
                    EndPoints = { ConnectionString }
                });
            return redis.GetDatabase();
        }
    }
}
