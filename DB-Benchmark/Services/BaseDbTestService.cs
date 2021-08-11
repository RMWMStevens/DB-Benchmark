using DB_Benchmark.Helpers;
using DB_Benchmark.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DB_Benchmark.Services
{
    public abstract class BaseDbTestService : BaseDbService
    {
        protected static List<string> SearchTerms { get; set; } = new();

        public static string GetTestsFilePath(TestProfile testProfile)
        {
            return $"./DB-Benchmark - Test - Keywords/{(int)testProfile}.txt";
        }

        public virtual async Task LoadTest(TestProfile testProfile)
        {
            var loadResult = await FileHelper.LoadAsync<string>(GetTestsFilePath(testProfile));

            if (!loadResult.IsSuccess)
            {
                LogHelper.LogError($"Loading test failed, {loadResult.Message}", System.ToString());
                throw new Exception();
            }

            SearchTerms = loadResult.Data.Split(new[] { Environment.NewLine }, StringSplitOptions.None).ToList();
        }

        public void RunTestChecks()
        {
            if (string.IsNullOrEmpty(ConnectionString))
            {
                LogHelper.LogError("No connection string given", System.ToString());
                throw new ArgumentNullException();
            }

            if (SearchTerms.Count < 1)
            {
                LogHelper.LogError("No search terms loaded, test cancelled.", System.ToString());
                throw new NullReferenceException();
            }
        }

        public async Task<T[]> RunTest<T>(object queriesObject)
        {
            RunTestChecks();

            var queries = (List<Task<T>>)queriesObject;
            return await Task.WhenAll(queries);
        }

        public abstract Task RunTest(object queriesObject);

        public abstract object SearchTermsToQueryTasks();
    }
}
