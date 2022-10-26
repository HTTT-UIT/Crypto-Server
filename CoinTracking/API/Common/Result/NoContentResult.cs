using API.Common.Enums;

namespace API.Common.Result
{
    public class NoContentResult : OperationResult
    {
        public NoContentResult()
            : base(OperationResultStatusCode.NoContent)
        { }
    }
}