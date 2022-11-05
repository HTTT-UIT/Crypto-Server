namespace API.Common.Result
{
    public class Result
    {
        public Result()
        {
        }

        public Result(int errorCode, string message)
        {
            ErrorCode = errorCode;
            Message = message;
        }

        public bool Succeeded => ErrorCode == 0;
        public int ErrorCode { get; set; }
        public string Message { get; set; } = string.Empty;

        public static Result Ok() => new();

        public static Result<T> Ok<T>(T value) => new(value);

        public static Result Failed(int errorCode, string message) => new(errorCode, message);
    }
}