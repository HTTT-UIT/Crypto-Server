using API.Common.Enums;

namespace API.Common.Result
{
    public class CreatedResult<TValue> : OperationResult<TValue>
    {
        public string Id { get; set; } = string.Empty;

        public CreatedResult(TValue value)
            : base(OperationResultStatusCode.Created, value)
        {
            if (value == null)
                throw new ArgumentNullException(nameof(value));
        }

        public CreatedResult(string id, TValue value)
            : this(value)
        {
            Id = id;
        }

        public CreatedResult(Guid id, TValue value)
            : this(value)
        {
            Id = id.ToString();
        }

        public CreatedResult(int id, TValue value)
            : this(value)
        {
            Id = id.ToString();
        }
    }
}