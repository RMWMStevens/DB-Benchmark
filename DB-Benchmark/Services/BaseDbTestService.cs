﻿using DB_Benchmark.Helpers;
using DB_Benchmark.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DB_Benchmark.Services
{
    public abstract class BaseDbTestService : BaseDbService
    {
        protected static List<string> searchTerms;

        public BaseDbTestService()
        {
            searchTerms = new List<string>();
        }

        public string GetTestsFilePath(TestProfile testSize)
        {
            return $"./DB-Benchmark - Test - Keywords/{(int)testSize}.txt";
        }

        public async Task LoadTest(TestProfile testSize)
        {
            var loadResult = await FileHelper.LoadAsync<string>(GetTestsFilePath(testSize));

            if (!loadResult.IsSuccess)
            {
                LogHelper.LogError($"Loading test failed, {loadResult.Message}", system.ToString());
                throw new Exception();
            }

            searchTerms = loadResult.Data.Split(new[] { Environment.NewLine }, StringSplitOptions.None).ToList();
        }

        public void RunTestChecks()
        {
            if (string.IsNullOrEmpty(connectionString))
            {
                LogHelper.LogError("No connection string given", system.ToString());
                throw new ArgumentNullException();
            }

            if (searchTerms.Count < 1)
            {
                LogHelper.LogError("No search terms loaded, test cancelled.", system.ToString());
                throw new NullReferenceException();
            }
        }

        public async Task RunTest<T>(object queriesObject)
        {
            RunTestChecks();

            var queries = (List<Task<T>>)queriesObject;
            await Task.WhenAll(queries);
        }

        public abstract Task RunTest(object queriesObject);

        public abstract object SearchTermsToQueryTasks();
    }
}
