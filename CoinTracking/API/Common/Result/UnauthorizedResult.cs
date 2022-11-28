using API.Common.Enums;

namespace API.Common.Result
{
    public class UnauthorizedResult : OperationResult
    {
        public UnauthorizedResult()
            : base(OperationResultStatusCode.Unauthorized)
        { }

        public UnauthorizedResult(string title, string detail)
            : base(OperationResultStatusCode.Unauthorized, title, detail)
        { }
    }
}