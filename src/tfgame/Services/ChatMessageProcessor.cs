using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using TT.Domain.Procedures;
using TT.Domain.Queries.DMRoll;
using TT.Domain.Statics;

namespace TT.Web.Services
{
    public static class ChatMessageProcessor
    {
        public static MessageOutput ProcessMessage(MessageData data)
        {
            var processors = new List<MessageProcessingTask>
            {
                new ReservedTextProcessor(),
                new DmMessageTextProcessor(),
                new DmActionTextProcessor(),
                new PlayerActionTextProcessor(),
                new RollTextProcessor(),
                new RegularTextProcessor() // Must be called last to handle regular messages
            };

            foreach (var processor in processors)
            {
                processor.Process(data);
                if (data.Processed)
                    return data.Output;
            }

            return new MessageOutput(string.Empty, MessageType.RegularText);
        }
    }

    public class MessageData
    {
        public string Name { get; private set; }
        public string Message { get; private set; }
        public bool Processed { get; private set; }

        public MessageOutput Output { get; set; }
        
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

    public class MessageOutput
    {
        public string Text { get; private set; }
        public MessageType MessageType { get; private set; }
        public bool SendPlayerChatColor { get; private set; }

        public MessageOutput(string text, MessageType messageType, bool sendPlayerChatColor = true)
        {
            Text = text;
            MessageType = messageType;
            SendPlayerChatColor = sendPlayerChatColor;
        }
    }

    public enum MessageType
    {
        RegularText,
        ReservedText,
        DmMessage,
        DmAction,
        Action,
        DieRoll,
        Notification,
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
            data.Output = new MessageOutput(data.Message, MessageType.RegularText);
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
            return ChatStatics.ReservedText.Any(reservedText => data.Message.StartsWith(reservedText));
        }

        protected override void ProcessInternal(MessageData data)
        {
            if (allowedRoles.Any(allowedRole => HttpContext.Current.User.IsInRole(allowedRole)))
            {
                data.Output = new MessageOutput(data.Message, MessageType.ReservedText);
                data.MarkAsProcessed();
                return;
            }

            var output = ChatStatics.ReservedText.Aggregate(data.Message, (current, reservedText) => current.Replace(reservedText, " "));
            data.Output = new MessageOutput(output, MessageType.RegularText);
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

            data.Output = new MessageOutput(output, MessageType.DmMessage);
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
            data.Output = new MessageOutput(cmd.Find(), MessageType.DmAction);
            data.MarkAsProcessed();
        }
    }

    public class RollTextProcessor : MessageProcessingTask
    {
        private const string regex = @"/roll (\d?\d?)d([0-9]*)(\+|\-?)(\d*)";
        
        protected override bool CanHandle(MessageData data)
        {
            var match = Regex.Match(data.Message, regex);
            var diceNumber = match.Groups[1].Length > 0 ? Convert.ToInt32(match.Groups[1].Value) : 1;

            return match.Success && !string.IsNullOrWhiteSpace(match.Groups[2].Value) && (diceNumber >= 1 && diceNumber <= 10);
        }

        protected override void ProcessInternal(MessageData data)
        {
            var match = Regex.Match(data.Message, regex);
            var sides = Convert.ToInt32(match.Groups[2].Value);
            var diceNumber = match.Groups[1].Length > 0 ? Convert.ToInt32(match.Groups[1].Value) : 1;
            var modifierOperand = match.Groups[3].Value;
            var modifier = match.Groups[4].Value;

            var results = GetDiceRolls(sides, diceNumber).ToList();

            var sb = new StringBuilder();
            foreach (var roll in results)
                sb.AppendFormat("{0}+", roll);

            var combined = sb.ToString().TrimEnd('+');
            var sumResult = SumResult(results, modifierOperand, modifier);
            var output = string.Format(" rolled {0}d{1}{2}{3}: {4} ({5})", diceNumber, sides, modifierOperand, modifier, combined, sumResult);

            data.Output = new MessageOutput(output, MessageType.DieRoll, false);
            data.MarkAsProcessed();
        }

        private static int SumResult(IEnumerable<int> results, string modifierOperand, string modifier)
        {
            var sum = results.Sum();

            if (modifierOperand.Length != 1 || modifier.Length <= 0)
                return sum;

            if (modifierOperand == "+")
                sum += Convert.ToInt32(Convert.ToInt32(modifier));
            else if (modifierOperand == "-")
                sum -= Convert.ToInt32(Convert.ToInt32(modifier));

            return sum;
        }

        private IEnumerable<int> GetDiceRolls(int sides, int dice)
        {
            for (var i = 0; i < dice; i++)
                yield return PlayerProcedures.RollDie(sides);
        }
    }

    public class PlayerActionTextProcessor : MessageProcessingTask
    {
        protected override bool CanHandle(MessageData data)
        {
            return data.Message.StartsWith("/me");
        }

        protected override void ProcessInternal(MessageData data)
        {
            var output = data.Message.Replace("/me", "");

            data.Output = new MessageOutput(output, MessageType.Action);
            data.MarkAsProcessed();
        }
    }
}