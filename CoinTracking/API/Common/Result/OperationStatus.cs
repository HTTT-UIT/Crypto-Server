using API.Common.Enums;

namespace API.Common.Result
{
    public class OperationStatus
    {
        public OperationStatus(OperationResultStatusCode statusCode) => StatusCode = statusCode;

        public OperationStatus(OperationResultStatusCode statusCode, string title, string? detail)
            : this(statusCode)
        {
            Title = title;
            Detail = detail;
        }

        public OperationResultStatusCode StatusCode { get; set; }

        public string Title { get; set; } = string.Empty;

        public string? Detail { get; set; }

        public bool HasDetails() => string.IsNullOrWhiteSpace(Title) == false || string.IsNullOrWhiteSpace(Detail) == false;
    }
}