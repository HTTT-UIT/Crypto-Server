using API.Common.Enums;

namespace API.Common.Result
{
    public class NotFoundResult : OperationResult
    {
        public NotFoundResult()
            : base(OperationResultStatusCode.NotFound, "Resource not found", "The resource you have requested cannot be found")
        { }

        public NotFoundResult(string title, string detail)
            : base(OperationResultStatusCode.NotFound, title, detail)
        { }
    }
}