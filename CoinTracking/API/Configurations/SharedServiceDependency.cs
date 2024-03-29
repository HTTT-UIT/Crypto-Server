﻿using API.Features.Shared.Services;

namespace API.Configurations
{
    public static class SharedServiceDependency
    {
        public static IServiceCollection AddSharedServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddTransient<ITokenService, TokenService>();
            services.AddScoped<IUserService, UserService>();
            return services;
        }
    }
}