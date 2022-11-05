using API.Common.Enums;
using System.ComponentModel.DataAnnotations;

namespace API.Common.Result
{
    public class BadRequestResult : OperationResult
    {
        public BadRequestResult()
            : base(OperationResultStatusCode.BadRequest)
        { }

        public BadRequestResult(string title, string? detail)
            : base(OperationResultStatusCode.BadRequest, title, detail)
        { }

        public BadRequestResult(IEnumerable<ValidationResult> validationResults)
           : base(OperationResultStatusCode.BadRequest) => ValidationResults = validationResults;

        public IEnumerable<ValidationResult> ValidationResults { get; } = Array.Empty<ValidationResult>();
    }
}