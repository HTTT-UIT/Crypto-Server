using API.Common.Enums;
using System.ComponentModel.DataAnnotations;

namespace API.Common.Result
{
    public class BadRequestResult<TValue> : OperationResult<TValue>
    {
        public BadRequestResult(IEnumerable<ValidationResult> validationResults, TValue value = default)
            : base(OperationResultStatusCode.BadRequest, value)
            => ValidationResults = validationResults;

        public IEnumerable<ValidationResult> ValidationResults { get; } = new ValidationResult[] { };
    }
}