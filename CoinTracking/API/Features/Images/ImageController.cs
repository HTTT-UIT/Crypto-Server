using API.Features.Shared;
using API.Features.Shared.Services;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace API.Features.Images
{
    [Route("api/[controller]")]
    public class ImageController : ApiControllerBase
    {
        private readonly IFileService _fileService;
        public ImageController(IMediator mediator, IFileService fileService) : base(mediator)
        {
            _fileService = fileService;
        }

        [HttpPost]
        public async Task<string> UploadImage(IFormFile form)
        {
            using var mem = new MemoryStream();
            if (mem == null  || form == null)
            {
                return string.Empty;
            }
            form.CopyTo(mem);
            var extension = Path.GetExtension(form.FileName);
            var imageName = Path.Combine(Guid.NewGuid().ToString(), extension);
            mem.Position = 0;
            return await _fileService.UploadAsync(imageName, mem);
        }
    }
}
