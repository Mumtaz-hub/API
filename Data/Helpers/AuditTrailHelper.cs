using System;
using System.Collections.Generic;
using Common.Enum;
using Domain.Entities;
using Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata;
using Newtonsoft.Json;

namespace Data.Helpers
{
    public class AuditTrailHelper
    {
        public EntityEntry Entry { get; }
        public AuditType AuditType { get; set; }
        public string UserId { get; set; }
        public string TableName { get; set; }
        public Dictionary<string, object> KeyValues { get; } = new();
        public Dictionary<string, object> OldValues { get; } = new();
        public Dictionary<string, object> NewValues { get; } = new();
        public List<string> ChangedColumns { get; } = new();

        public AuditTrailHelper(EntityEntry entry, string userId)
        {
            Entry = entry;
            UserId = userId;
            SetChanges();
        }

        private void SetChanges()
        {
            TableName = Entry.Metadata.GetTableName();
            foreach (var property in Entry.Properties)
            {
                var storeObjectId = StoreObjectIdentifier.Create(property.Metadata.DeclaringEntityType, StoreObjectType.Table);

                var propertyName = property.Metadata.Name;
                var dbColumnName = property.Metadata.GetColumnName(storeObjectId.GetValueOrDefault());

                if (property.Metadata.IsPrimaryKey())
                {
                    KeyValues[propertyName] = property.CurrentValue;
                    continue;
                }

                switch (Entry.State)
                {
                    case EntityState.Added:
                        NewValues[propertyName] = property.CurrentValue;
                        AuditType = AuditType.Create;
                        break;

                    case EntityState.Deleted:
                        OldValues[propertyName] = property.OriginalValue;
                        AuditType = AuditType.Delete;
                        break;

                    case EntityState.Modified:
                        if (property.IsModified)
                        {
                            ChangedColumns.Add(dbColumnName);

                            OldValues[propertyName] = property.OriginalValue;
                            NewValues[propertyName] = property.CurrentValue;
                            AuditType = AuditType.Update;
                        }
                        break;
                    case EntityState.Detached:
                        break;
                    case EntityState.Unchanged:
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        public AuditTrail ToAudit(Guid id = new())
        {
            var audit = new AuditTrail
            {
                Id = id,
                AuditDateTimeUtc = DateTime.UtcNow,
                AuditType = AuditType,
                UserId = UserId,
                TableName = TableName,
                KeyValues = JsonConvert.SerializeObject(KeyValues),
                OldValues = OldValues.Count == 0 ? null : OldValues.ToJson(),
                NewValues = NewValues.Count == 0 ? null : NewValues.ToJson(),
                AffectedColumns = ChangedColumns.Count == 0 ? null : ChangedColumns.ToJson()
            };

            return audit;
        }
    }
}
