using API.Common.Enums;

namespace API.Common.Result
{
    public class ConflictResult : OperationResult
    {
        public ConflictResult()
            : base(OperationResultStatusCode.Conflict)
        { }

        public ConflictResult(string title, string detail)
            : base(OperationResultStatusCode.Conflict, title, detail)
        { }
    }
}