using System.Security.Claims;

namespace API.Features.Shared.Models
{
    public class ApplicationUser : IApplicationUser
    {
        private readonly IHttpContextAccessor _httpContext;
        public ApplicationUser(IHttpContextAccessor httpContext)
        {
            _httpContext = httpContext;

            var userId = _httpContext.HttpContext?.User.Claims.Where(c => c.Type == ClaimTypes.PrimarySid)
                .Select(c => c.Value).SingleOrDefault() ?? string.Empty;

            bool isValid = Guid.TryParse(userId, out Guid userGuid);
            if (userId != null && isValid)
            {
                UserId = userGuid;
            }
        }

        public Guid UserId { get; set; } = Guid.Empty;
    }
}
