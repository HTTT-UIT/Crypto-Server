namespace API.Features.Shared.Services
{
    public interface IFileService
    {
        Task<Stream> OpenReadAsync(string containerName, string objectId, CancellationToken cancellationToken = default);

        Task UploadAsync(string containerName, string objectId, Stream stream, CancellationToken cancellationToken = default);
    }
}
