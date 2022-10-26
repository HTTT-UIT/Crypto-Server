namespace API.Common.Result
{
    public interface IOperationResult
    {
        OperationStatus Status { get; }
        bool Succeeded { get; }
    }
}