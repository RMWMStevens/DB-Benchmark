using DB_Benchmark.Helpers;
using DB_Benchmark.Models.Enums;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
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
                LogHelper.Log($"Converting search words to queries for {dbService.System} - {testType} test", nameof(TestService));
                var queriesObject = dbService.SearchTermsToQueries();
                LogHelper.Log($"Starting {dbService.System} - {testType} test", nameof(TestService));
                await dbService.RunTest(queriesObject);
                LogHelper.Log($"Finished {dbService.System} - {testType} test, time: {stopwatch.Elapsed}", nameof(TestService));
            }
        }

        async Task LoadTests(TestSize testType)
        {
            await dbServices.FirstOrDefault().LoadTest(testType);
        }
    }
}
