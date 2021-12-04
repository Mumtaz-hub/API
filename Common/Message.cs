using System;
using System.Collections.Generic;
using Common.Interface;
using Newtonsoft.Json;

namespace Common
{
    public class Message
    {
        public Message() { }

        public Message(Message parentMessage) : this(parentMessage.User)
        {
            MessageId = parentMessage.MessageId;
        }

        public Message(ILoggedOnUserProvider user)
        {
            SetUser(user);
        }

        [JsonIgnore]
        public ILoggedOnUserProvider User { get; private set; }

        [JsonIgnore]
        public long LoggedOnUserId { get; set; }

        [JsonIgnore]
        public Guid MessageId { get; set; } = Guid.NewGuid();

        [JsonIgnore]
        public DateTime TimeStamp { get; set; } = DateTime.UtcNow;

        [JsonIgnore]
        public List<string> MessageExecutionLogs { get; set; } = new List<string>();

        [JsonIgnore]
        public bool AuditThisMessage { get; private set; } = true;

        public void SetUser(ILoggedOnUserProvider user)
        {
            User = user;
            LoggedOnUserId = User?.UserId ?? 0;
        }

        public void DontAuditThisMessage()
        {
            AuditThisMessage = false;
        }
    }
}
