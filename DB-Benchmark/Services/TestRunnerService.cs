﻿using DB_Benchmark.Disposables;
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
        readonly List<BaseDbTestService> dbServices;
        public List<BaseDbTestService> DbServices { get { return dbServices; } }

        public TestRunnerService()
        {
            dbServices = new List<BaseDbTestService>
            {
                new MongoTestService(),
                new MsSqlTestService(),
            };
        }

        static string GetTestResultsFilePath(string fileName)
        {
            return $"./DB-Benchmark - Test - Results/{fileName}";
        }

        public async Task<ActionResult> RunTestSuite(bool runWarmupTests)
        {
            try
            {
                var testTypes = (TestProfile[])Enum.GetValues(typeof(TestProfile));

                if (runWarmupTests)
                {
                    LogHelper.Log("Running warm-up tests for all test types... Sit back and relax, this can take a while.");

                    foreach (var testType in testTypes)
                    {
                        await LoadTest(testType);
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

                    ShowTestTypes(testTypes);

                    LogHelper.Log($"Test results will be saved to {GetTestResultsFilePath(fileName)}\n");

                    foreach (var testType in testTypes)
                    {
                        await LoadTest(testType);
                        await RunTests(testType);
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

        async Task LoadTest(TestProfile testType)
        {
            await dbServices.FirstOrDefault().LoadTest(testType);
        }

        async Task RunWarmupTests()
        {
            foreach (var dbService in dbServices)
            {
                var queriesObject = dbService.SearchTermsToQueryTasks();
                await dbService.RunTest(queriesObject);
                await Rest(1000);
            }
        }

        async Task RunTests(TestProfile testType)
        {
            var stopwatch = new Stopwatch();
            LogHelper.Log("------------------------------------------------");
            LogHelper.Log($"Profile: {testType}\n");

            foreach (var dbService in dbServices)
            {
                stopwatch.Restart();
                LogHelper.Log($"{dbService.System} - Running...");
                var queriesObject = dbService.SearchTermsToQueryTasks();
                await dbService.RunTest(queriesObject);
                LogHelper.Log($"{dbService.System} - Completed. Time: {stopwatch.Elapsed}");

                LogHelper.Log("Resting...\n");
                await Rest(5000);
            }
        }

        private static async Task Rest(int milliseconds)
        {
            await Task.Delay(milliseconds);
        }

        private static void ShowTestTypes(TestProfile[] testTypes)
        {
            LogHelper.Log($"Profile(s): {string.Join(" | ", testTypes)}\n");

            foreach (var testType in testTypes)
            {
                LogHelper.Log($"Profile '{testType}': {(int)testType} search queries");
            }
        }
    }
}