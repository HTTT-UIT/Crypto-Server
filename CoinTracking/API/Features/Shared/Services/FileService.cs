using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;

namespace API.Features.Shared.Services
{
    public class FileService : IFileService
    {
        private readonly BlobServiceClient _client;
        private readonly object _lock = new();

        private const string _container = "mycontainer";

        public FileService(BlobServiceClient client) => _client = client;

        public Task<Stream> OpenReadAsync(string blobName, CancellationToken cancellationToken = default)
        {
            var containerClient = _client.GetBlobContainerClient(_container);
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

        public async Task<string> UploadAsync(string blobName, Stream stream, CancellationToken cancellationToken = default)
        {
            var containerClient = _client.GetBlobContainerClient(_container);
            if (!containerClient.Exists(cancellationToken))
            {
                lock (_lock)
                {
                    containerClient.Create(cancellationToken: cancellationToken);
                }
            }

            var blobClient = containerClient.GetBlobClient(blobName);

            await blobClient.UploadAsync(stream, cancellationToken);

            return blobClient.Uri.AbsoluteUri;
        }
    }
}
