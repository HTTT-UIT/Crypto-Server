using API.Common.Enums;

namespace API.Common.Result
{
    public class ForbiddenResult : OperationResult
    {
        public ForbiddenResult()
            : base(OperationResultStatusCode.Forbidden)
        { }

        public ForbiddenResult(string title, string detail)
            : base(OperationResultStatusCode.Forbidden, title, detail)
        { }
    }
}