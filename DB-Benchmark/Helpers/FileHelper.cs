using DB_Benchmark.Models;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading.Tasks;

namespace DB_Benchmark.Helpers
{
    public static class FileHelper
    {
        public static bool Exists(string filePath)
        {
            return File.Exists(filePath);
        }

        public async static Task<ActionResult> SaveAsync(string filePath, string connectionInfo)
        {
            try
            {
                new FileInfo(filePath).Directory.Create();
                await File.WriteAllTextAsync(filePath, connectionInfo);
                return new ActionResult { IsSuccess = true };
            }
            catch (System.Exception ex)
            {
                return ActionResultHelper.CreateErrorResult<string>(ex);
            }
        }

        public static async Task<ActionResult<string>> LoadAsync<T>(string filePath)
        {
            try
            {
                if (!Exists(filePath)) { return ActionResultHelper.CreateFailureResult<string>("File does not exist!"); }

                var result = await File.ReadAllTextAsync(filePath);
                return ActionResultHelper.CreateSuccessResult(result);
            }
            catch (System.Exception ex)
            {
                return ActionResultHelper.CreateErrorResult<string>(ex);
            }
        }
    }
}
