using API.Common.Enums;

namespace API.Common.Result
{
    public class NotAcceptedResult : OperationResult
    {
        public NotAcceptedResult()
            : base(OperationResultStatusCode.NotAcceptable)
        { }
    }
}