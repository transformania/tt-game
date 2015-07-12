using tfgame.Procedures;

namespace tfgame.dbModels.Commands.ChatLog
{
    public class WriteLogToDatabase : Command
    {
        public string Room { get; set; }
        public string Name { get; set; }
        public string Message { get; set; }
        
        internal override void InternalExecute()
        {
            ChatLogProcedures.WriteLogToDatabase(Room, Name, Message);
        }
    }
}