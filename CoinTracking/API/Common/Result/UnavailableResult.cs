using API.Common.Enums;

namespace API.Common.Result
{
    public class UnavailableResult : OperationResult
    {
        public UnavailableResult()
            : base(OperationResultStatusCode.Unavailable)
        { }

        public UnavailableResult(string title, string detail)
            : base(OperationResultStatusCode.Unavailable, title, detail)
        { }
    }
}