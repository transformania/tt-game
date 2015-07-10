using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using tfgame.dbModels.Queries.DMRoll;
using tfgame.Procedures;
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
            return ChatStatics.ReservedText.Any(reservedText => data.Message.Contains(reservedText));
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

    public class DmMessageTextProcessor : MessageProcessingTask
    {
        protected override bool CanHandle(MessageData data)
        {
            return data.Message.StartsWith("/dm message");
        }

        protected override void ProcessInternal(MessageData data)
        {
            var output = data.Message.Replace("/dm message", "");

            data.Output = string.Format("[=[{0} [DM]:  {1}]=]", data.Name , output);
            data.MarkAsProcessed();
        }
    }

    public class DmActionTextProcessor : MessageProcessingTask
    {
        private const string regex = @"\/dm ([a-zA-Z]|.*):([a-zA-Z]*)";
        
        protected override bool CanHandle(MessageData data)
        {
            var match = new Regex(regex).Match(data.Message);

            var actionType = match.Groups[1].Value;
            var tag = match.Groups[2].Value;

            return match.Success && ChatStatics.ActionTypes.Contains(actionType) && ChatStatics.Tags.Contains(tag);
        }

        protected override void ProcessInternal(MessageData data)
        {
            var match = new Regex(regex).Match(data.Message);

            var cmd = new GetRollText { ActionType = match.Groups[1].Value, Tag = match.Groups[2].Value };
            data.Output = string.Format("[=[{0}]=]", cmd.Find());
            data.MarkAsProcessed();
        }
    }

    public class RollTextProcessor : MessageProcessingTask
    {
        private const string regex = @"/roll d([0-9]*)";
        
        protected override bool CanHandle(MessageData data)
        {
            var match = Regex.Match(data.Message, regex);
            return match.Success && !string.IsNullOrWhiteSpace(match.Groups[1].Value);
        }

        protected override void ProcessInternal(MessageData data)
        {
            var match = Regex.Match(data.Message, regex);
            var die = Convert.ToInt32(match.Groups[1].Value);

            data.Output = string.Format("[-[{0} rolled a {1} (d{2}).]-]", data.Name, PlayerProcedures.RollDie(die), die);
            data.MarkAsProcessed();
        }
    }
}