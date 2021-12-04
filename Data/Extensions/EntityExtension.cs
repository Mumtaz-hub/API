using System;
using Common.Enum;
using Domain.Entities;

namespace Data.Extensions
{
    public static class EntityExtension
    {
        public static void PopulateMetaData(this BaseEntity entity, string currentUserId, StatusType statusType = StatusType.Enabled)
        {
            if (entity.Id > 0)
            {
                //edit
                entity.LastChangeUserId = currentUserId;
                entity.LastChangeTs = DateTime.UtcNow;
            }
            else
            {
                //add
                entity.CreationUserId = currentUserId;
                entity.CreationTs = DateTime.UtcNow;
                entity.LastChangeUserId = currentUserId;
                entity.LastChangeTs = DateTime.UtcNow;
            }

            entity.Status = statusType;
        }
    }
}
