using System.Linq;
using tfgame.dbModels.Abstract;
using tfgame.dbModels.Models;

namespace tfgame.dbModels.Concrete
{
    public class EFPlayerRepository : IPlayerRepository
    {
        private StatsContext context = new StatsContext();

        public IQueryable<Player> Players
        {
            get { return context.Players; }
        }

        public IQueryable<DbStaticForm> DbStaticForms
        {
            get { return context.DbStaticForms; }
        }

        public void SavePlayer(Player Player)
        {
            if (Player.Id == 0)
            {
                context.Players.Add(Player);
            }
            else
            {
                Player editMe = context.Players.Find(Player.Id);
                if (editMe != null)
                {
                    // dbEntry.Name = Player.Name;
                    // dbEntry.Message = Player.Message;
                    // dbEntry.TimeStamp = Player.TimeStamp;

                }
            }
            context.SaveChanges();
        }

        public void DeletePlayer(int id)
        {

            Player dbEntry = context.Players.Find(id);
            if (dbEntry != null)
            {
                context.Players.Remove(dbEntry);
                context.SaveChanges();
            }
        }

        public void SaveDbStaticForm(DbStaticForm DbStaticForm)
        {
            if (DbStaticForm.Id == 0)
            {
                context.DbStaticForms.Add(DbStaticForm);
            }
            else
            {
                DbStaticForm editMe = context.DbStaticForms.Find(DbStaticForm.Id);
                if (editMe != null)
                {
                    // dbEntry.Name = DbStaticForm.Name;
                    // dbEntry.Message = DbStaticForm.Message;
                    // dbEntry.TimeStamp = DbStaticForm.TimeStamp;

                }
            }
            context.SaveChanges();
        }

        public void DeleteDbStaticForm(int id)
        {

            DbStaticForm dbEntry = context.DbStaticForms.Find(id);
            if (dbEntry != null)
            {
                context.DbStaticForms.Remove(dbEntry);
                context.SaveChanges();
            }
        }

    }
}