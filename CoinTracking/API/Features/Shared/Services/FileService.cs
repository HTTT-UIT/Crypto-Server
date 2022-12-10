using Azure.Storage.Blobs;

namespace API.Features.Shared.Services
{
    public class FileService : IFileService
    {
        private readonly BlobServiceClient _client;
        private readonly object _lock = new();

        public FileService(BlobServiceClient client) => _client = client;

        public Task<Stream> OpenReadAsync(string containerName, string blobName, CancellationToken cancellationToken = default)
        {
            var containerClient = _client.GetBlobContainerClient(containerName);
            if (containerClient.Exists(cancellationToken))
            {
                var blobClient = containerClient.GetBlobClient(blobName);
                if (blobClient.Exists(cancellationToken))
                {
                    return blobClient.OpenReadAsync(cancellationToken: cancellationToken);
                }
            }

            return Task.FromResult(Stream.Null);
        }

        public Task UploadAsync(string containerName, string blobName, Stream stream, CancellationToken cancellationToken = default)
        {
            var containerClient = _client.GetBlobContainerClient(containerName);
            if (!containerClient.Exists(cancellationToken))
            {
                lock (_lock)
                {
                    containerClient.Create(cancellationToken: cancellationToken);
                }
            }

            var blobClient = containerClient.GetBlobClient(blobName);

            return blobClient.UploadAsync(stream, cancellationToken);
        }
    }
}
