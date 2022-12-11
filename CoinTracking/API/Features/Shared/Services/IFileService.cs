namespace API.Features.Shared.Services
{
    public interface IFileService
    {
        Task<Stream> OpenReadAsync(string objectId, CancellationToken cancellationToken = default);

        Task<string> UploadAsync(string objectId, Stream stream, CancellationToken cancellationToken = default);
    }
}
