﻿using API.Infrastructure.Entities;

namespace API.Common.Commands
{
    public class BaseHandle
    {
        public void BaseCreate<T1, T2>(T1 entity, T2 command) where T1 : BaseEntity where T2 : BaseCommand
        {
            entity.CreatedBy = command.ProfileId == Guid.Empty ? string.Empty : command.ProfileId.ToString();
            entity.CreatedAt = DateTime.Now;
        }

        public void BaseUpdate<T1, T2> (T1 entity, T2 command) where T1 : BaseEntity where T2 : BaseCommand
        {
            entity.LastUpdatedBy = command.ProfileId == Guid.Empty ? string.Empty : command.ProfileId.ToString();
            entity.LastUpdatedAt = DateTime.Now;
        }
    }
}