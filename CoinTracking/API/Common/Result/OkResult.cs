using API.Common.Enums;

namespace API.Common.Result
{
    public class OkResult : OperationResult
    {
        public OkResult()
            : base(OperationResultStatusCode.Ok)
        { }
    }

    public class OkResult<TValue> : OperationResult<TValue>
    {
        public OkResult(TValue value)
            : base(OperationResultStatusCode.Ok, value)
        {
            if (value == null)
                throw new ArgumentNullException(nameof(value));

            Value = value;
        }
    }
}