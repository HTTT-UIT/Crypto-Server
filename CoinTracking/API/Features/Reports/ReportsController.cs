using API.Features.Reports.Commands;
using API.Features.Reports.Queries;
using API.Features.Shared;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace API.Features.Reports
{
    [Route("api/[controller]")]
    public class ReportsController : ApiControllerBase
    {
        public ReportsController(IMediator mediator) : base(mediator)
        {}

        [HttpPost]
        public Task<IActionResult> Create(Create.Command command)
            => HandleRequest(command);

        [HttpPut("{id}")]
        public Task<IActionResult> UpdateStatus(UpdateStatus.Command command)
            => HandleRequest(command);

        [HttpGet("{id}")]
        public Task<IActionResult> Get(Get.Query query)
            => HandleRequest(query);

        [HttpGet]
        public Task<IActionResult> List(List.Query query)
            => HandleRequest(query);
    }
}
