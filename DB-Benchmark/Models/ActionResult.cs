namespace DB_Benchmark.Models
{
    public class ActionResult
    {
        public bool IsSuccess { get; set; }
        public string Message { get; set; }
    }

    public class ActionResult<T> : ActionResult
    {
        public T Data { get; set; }
    }
}
