using API.Common.Enums;

namespace API.Common.Result
{
    public class AcceptedResult : OperationResult
    {
        public AcceptedResult()
            : base(OperationResultStatusCode.Accepted)
        { }
    }

    public class AcceptedResult<TValue> : OperationResult<TValue>
    {
        public AcceptedResult(TValue value)
            : base(OperationResultStatusCode.Accepted, value)
        {
            if (value == null)
                throw new ArgumentNullException(nameof(value));

            Value = value;
        }
    }
}