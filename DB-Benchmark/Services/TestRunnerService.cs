using DB_Benchmark.Disposables;
using DB_Benchmark.Helpers;
using DB_Benchmark.Models;
using DB_Benchmark.Models.Enums;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace DB_Benchmark.Services
{
    public class TestRunnerService
    {
        public List<BaseDbTestService> DbServices { get; }

        public TestRunnerService()
        {
            DbServices = new List<BaseDbTestService>
            {
                new RedisTestService(), // Needs to be at the top, otherwise caching its test won't work properly!
                //new MongoTestService(),
                //new MsSqlTestService(),
                //new Neo4jTestService(),
            };
        }

        static string GetTestResultsFilePath(string fileName)
        {
            return $"./DB-Benchmark - Test - Results/{fileName}";
        }

        public async Task<ActionResult> RunTestSuite(bool runWarmupTests, int runCount)
        {
            try
            {
                var testProfiles = (TestProfile[])Enum.GetValues(typeof(TestProfile));

                if (runWarmupTests)
                {
                    LogHelper.Log("Running warm-up tests for all test types... Sit back and relax, this can take a while.\n");

                    foreach (var testProfile in testProfiles)
                    {
                        await LoadTest(testProfile);
                        await RunWarmupTests();
                    }

                    Console.Clear();
                }

                var fileNameSuffix = runWarmupTests ? "" : " - No warm-up";
                var fileName = (DateTime.Now.ToString("s")).Replace(':', '-') + $"{fileNameSuffix}.txt";

                using (var os = new OutputSaver(GetTestResultsFilePath(fileName)))
                {
                    if (!runWarmupTests)
                    {
                        LogHelper.LogError("Warning! Test results may be wildly inaccurate because the warm-up run was not performed.\n");
                    }

                    ShowTestProfiles(testProfiles);

                    LogHelper.Log($"Test results will be saved to {GetTestResultsFilePath(fileName)}\n");

                    foreach (var testProfile in testProfiles)
                    {
                        await LoadTest(testProfile);
                        await RunTests(testProfile, runCount);
                    }

                    LogHelper.Log("Finished.\n");
                }

                return new ActionResult { IsSuccess = true };
            }
            catch (Exception ex)
            {
                return ActionResultHelper.CreateErrorResult<string>(ex);
            }
        }

        async Task LoadTest(TestProfile testProfile)
        {
            await DbServices.FirstOrDefault().LoadTest(testProfile);
        }

        async Task RunWarmupTests()
        {
            foreach (var dbService in DbServices)
            {
                var queriesObject = dbService.SearchTermsToQueryTasks();
                await dbService.RunTest(queriesObject);
                await Rest(1000);
            }
        }

        async Task RunTests(TestProfile testProfile, int runCount)
        {
            var stopwatch = new Stopwatch();
            LogHelper.Log("------------------------------------------------");
            LogHelper.Log($"Profile: {testProfile}\n");

            for (var i = 1; i <= runCount; i++)
            {
                LogHelper.Log($"Run {i}\n");
                foreach (var dbService in DbServices)
                {
                    stopwatch.Restart();
                    LogHelper.Log($"{dbService.System} - Running...");
                    var queriesObject = dbService.SearchTermsToQueryTasks();
                    await dbService.RunTest(queriesObject);
                    LogHelper.Log($"{dbService.System} - Completed. Time: {stopwatch.Elapsed}");

                    LogHelper.Log("Resting...\n");
                    await Rest(2000);
                }
            }
        }

        private static async Task Rest(int milliseconds)
        {
            await Task.Delay(milliseconds);
        }

        private static void ShowTestProfiles(TestProfile[] testProfiles)
        {
            LogHelper.Log($"Profile(s): {string.Join(" | ", testProfiles)}\n");

            foreach (var testProfile in testProfiles)
            {
                LogHelper.Log($"Profile '{testProfile}': {(int)testProfile} search queries");
            }
        }
    }
}
