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
        protected static List<string> searchTerms;

        public BaseDbTestService()
        {
            searchTerms = new List<string>();
        }

        public string GetTestsFilePath(TestSize testSize)
        {
            return $"./DB-Benchmark - TestSearchTerms/{(int)testSize}.txt";
        }

        public async Task LoadTest(TestSize testSize)
        {
            var loadResult = await FileHelper.LoadAsync<string>(GetTestsFilePath(testSize));

            if (!loadResult.IsSuccess)
            {
                LogHelper.Warn($"Loading test failed, {loadResult.Message}", $"{nameof(BaseDbTestService)}({nameof(system)})");
                return;
            }

            searchTerms = loadResult.Data.Split(new[] { Environment.NewLine }, StringSplitOptions.None).ToList();
        }

        public virtual async Task RunTest(object queriesObject)
        {
            if (string.IsNullOrEmpty(connectionString))
            {
                LogHelper.Warn("No connection string given", $"{nameof(BaseDbTestService)}({nameof(system)})");
            }

            if (searchTerms.Count < 1)
            {
                LogHelper.Warn("No search terms found, test cancelled.", $"{nameof(BaseDbTestService)}({nameof(system)})");
                return;
            }
        }

        public abstract object SearchTermsToQueryTasks();
    }
}
