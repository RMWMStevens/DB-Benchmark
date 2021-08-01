using DB_Benchmark.Helpers;
using DB_Benchmark.Models.Enums;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;

namespace DB_Benchmark.Services
{
    public class TestService
    {
        readonly List<BaseDbTestService> dbServices;
        public List<BaseDbTestService> DbServices { get { return dbServices; } }

        public TestService()
        {
            dbServices = new List<BaseDbTestService>
            {
                //new MongoService(),
                new MsSqlService(),
            };
        }

        public async Task RunTestSuite()
        {
            var testTypes = (TestSize[])Enum.GetValues(typeof(TestSize));

            foreach (var testType in testTypes)
            {
                await LoadTests(testType);
                await RunTests(testType);
            }
        }

        async Task RunTests(TestSize testType)
        {
            var stopwatch = new Stopwatch();

            foreach (var dbService in dbServices)
            {
                stopwatch.Restart();
                LogHelper.Log($"Starting {dbService.System} {testType} test", nameof(TestService));
                await dbService.RunTest();
                LogHelper.Log($"Finished {dbService.System} {testType} test, time: {stopwatch.Elapsed}", nameof(TestService));
            }
        }

        async Task LoadTests(TestSize testType)
        {
            var loadTasks = new List<Task>();

            foreach (var dbService in dbServices)
            {
                loadTasks.Add(dbService.LoadTest(testType));
            }

            await Task.WhenAll(loadTasks);
        }
    }
}
