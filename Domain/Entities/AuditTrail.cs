using System;
using Common.Enum;

namespace Domain.Entities
{
    public class AuditTrail
    {
        public Guid Id { get; set; }                
        public DateTime AuditDateTimeUtc { get; set; }  
        public AuditType AuditType { get; set; }           
        public string UserId { get; set; }           
        public string TableName { get; set; }           
        public string KeyValues { get; set; }          
        public string OldValues { get; set; }          
        public string NewValues { get; set; }       
        public string AffectedColumns { get; set; }     
    }
}
