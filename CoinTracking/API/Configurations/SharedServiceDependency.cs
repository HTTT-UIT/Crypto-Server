using API.Features.Shared.Models;
using API.Features.Shared.Services;

namespace API.Configurations
{
    public static class SharedServiceDependency
    {
        public static IServiceCollection AddSharedServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddTransient<ITokenService, TokenService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<ICoinService, CoinService>();
            services.AddScoped<IFileService, FileService>();
            services.AddTransient<IApplicationUser, ApplicationUser>();
            return services;
        }
    }
}