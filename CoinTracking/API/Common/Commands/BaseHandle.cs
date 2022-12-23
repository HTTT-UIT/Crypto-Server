using API.Features.Shared.Models;
using API.Infrastructure.Entities.Common;

namespace API.Common.Commands
{
    public abstract class BaseHandle
    {
        private readonly IApplicationUser _applicationUser;

        public BaseHandle(IApplicationUser applicationUser)
        {
            _applicationUser = applicationUser;
        }

        public void BaseCreate<T1, T2>(T1 entity, T2 command) where T1 : BaseEntity where T2 : BaseCommand
        {
            entity.CreatedBy = _applicationUser.UserId.ToString();

            if (command.ProfileId != Guid.Empty)
            {
                entity.CreatedBy = command.ProfileId.ToString();
            }
            entity.CreatedAt = DateTime.Now;
        }

        public void BaseUpdate<T1, T2> (T1 entity, T2 command) where T1 : BaseEntity where T2 : BaseCommand
        {
            entity.LastUpdatedBy = _applicationUser.UserId.ToString();

            if (command.ProfileId != Guid.Empty)
            {
                entity.LastUpdatedBy = command.ProfileId.ToString();
            }

            entity.LastUpdatedAt = DateTime.Now;
        }

        public void BaseDelete<T1>(T1 entity) where T1 : ISoftEntity
        {
            entity.Deleted = true;
        }

        public void BaseDelete<T1, T2>(T1 entity, T2 command) where T1 : BaseEntity, ISoftEntity where T2 : BaseCommand
        {
            entity.Deleted = true;
            BaseUpdate(entity, command);
        }
    }
}
