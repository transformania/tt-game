using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using tfgame.dbModels.Abstract;
using tfgame.dbModels.Models;

namespace tfgame.dbModels.Concrete
{
    public class EFFriendRepository : IFriendRepository
    {
        private StatsContext context = new StatsContext();

        public IQueryable<Friend> Friends
        {
            get { return context.Friends; }
        }

        public void SaveFriend(Friend Friend)
        {
            if (Friend.Id == 0)
            {
                context.Friends.Add(Friend);
            }
            else
            {
                Friend editMe = context.Friends.Find(Friend.Id);
                if (editMe != null)
                {
                    // dbEntry.Name = Friend.Name;
                    // dbEntry.Message = Friend.Message;
                    // dbEntry.TimeStamp = Friend.TimeStamp;

                }
            }
            context.SaveChanges();
        }

        public void DeleteFriend(int id)
        {

            Friend dbEntry = context.Friends.Find(id);
            if (dbEntry != null)
            {
                context.Friends.Remove(dbEntry);
                context.SaveChanges();
            }
        }

    }
}