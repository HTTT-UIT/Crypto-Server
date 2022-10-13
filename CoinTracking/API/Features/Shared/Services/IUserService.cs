﻿using API.Infrastructure.Entities;

namespace API.Features.Shared.Services
{
    public interface IUserService
    {
        Task<List<UserEntity>> GetList();

        Task<UserEntity> GetByCredential(string userName, string password);
    }
}