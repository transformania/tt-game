using TT.Domain.Chat.Entities;

namespace TT.Tests.Builders.Chat
{
    public class ChatLogBuilder : Builder<ChatLog, int>
    {
        public ChatLogBuilder()
        {
            Instance = Create();
        }
    }
}