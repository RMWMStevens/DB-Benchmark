using DB_Benchmark.Models;
using System;

namespace DB_Benchmark.Helpers
{
    public static class ActionResultHelper
    {
        public static ActionResult<T> CreateFailureResult<T>(string message = default)
        {
            return new ActionResult<T>
            {
                IsSuccess = false,
                Message = message
            };
        }

        public static ActionResult<T> CreateSuccessResult<T>(T data, string message = default)
        {
            return new ActionResult<T>()
            {
                IsSuccess = true,
                Data = data,
                Message = message
            };
        }

        public static ActionResult<T> CreateErrorResult<T>(Exception ex)
        {
            return new ActionResult<T>()
            {
                IsSuccess = false,
                Message = ex.Message,
                Data = default
            };
        }
    }
}
