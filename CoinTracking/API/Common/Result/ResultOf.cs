namespace API.Common.Result
{
    public class Result<T>
    {
        public Result(T value)
        {
            Value = value;
        }

        public Result(Result result, T? value = default)
        {
            ErrorCode = result.ErrorCode;
            Message = result.Message;
            Value = value;
        }

        public bool Succeeded => ErrorCode == 0;
        public int ErrorCode { get; set; }
        public string Message { get; set; } = string.Empty;

        public T? Value { get; set; }

        public Result ToResult()
        {
            return new Result(ErrorCode, Message);
        }

        public static implicit operator Result<T>(Result result)
        {
            return new Result<T>(result);
        }
    }
}