using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using tfgame.Statics;

namespace tfgame.Services
{
    public static class ChatMessageProcessor
    {
        public static string ProcessMessage(MessageData data)
        {
            var processors = new List<MessageProcessingTask>
            {
                new ReservedTextProcessor(),
                new RegularTextProcessor() // Must be called last to handle regular messages
            };

            foreach (var processor in processors)
            {
                processor.Process(data);
                if (data.Processed)
                    return data.Output;
            }

            return string.Empty;
        }
    }

    public class MessageData
    {
        public string Name { get; private set; }
        public string Message { get; private set; }
        public bool Processed { get; private set; }
        
        public string Output { get; set; }
        
        public MessageData(string name, string message)
        {
            Name = name;
            Message = message;
        }

        public void MarkAsProcessed()
        {
            Processed = true;
        }
    }

    public abstract class MessageProcessingTask
    {
        protected abstract bool CanHandle(MessageData data);
        protected abstract void ProcessInternal(MessageData data);

        public void Process(MessageData data)
        {
            if (CanHandle(data))
                ProcessInternal(data);
        }
    }

    public class RegularTextProcessor : MessageProcessingTask
    {
        protected override bool CanHandle(MessageData data)
        {
            return !string.IsNullOrWhiteSpace(data.Name) && !string.IsNullOrWhiteSpace(data.Message);
        }

        protected override void ProcessInternal(MessageData data)
        {
            data.Output = string.Format("{0}   [.[{1}].]", data.Message, DateTime.UtcNow.ToShortTimeString());
            data.MarkAsProcessed();
        }
    }

    public class ReservedTextProcessor : MessageProcessingTask
    {
        private readonly IList<string> allowedRoles = new []
        {
            PvPStatics.Permissions_Admin, 
            PvPStatics.Permissions_Moderator
        };
        
        protected override bool CanHandle(MessageData data)
        {
            return true;
        }

        protected override void ProcessInternal(MessageData data)
        {
            if (allowedRoles.Any(allowedRole => HttpContext.Current.User.IsInRole(allowedRole)))
            {
                data.Output = string.Format("{0}   [.[{1}].]", data.Message, DateTime.UtcNow.ToShortTimeString());
                data.MarkAsProcessed();
                return;
            }

            var output = ChatStatics.ReservedText.Aggregate(data.Message, (current, reservedText) => current.Replace(reservedText, " "));
            data.Output = string.Format("{0}   [.[{1}].]", output, DateTime.UtcNow.ToShortTimeString());
            data.MarkAsProcessed();
        }
    }
}