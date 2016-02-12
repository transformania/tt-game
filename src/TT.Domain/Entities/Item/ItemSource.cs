using System;
using TT.Domain.Entities.Identity;

namespace TT.Domain.Entities.Items
{
    public class ItemSource : Entity<int>
    {

        public string dbName { get; private set; }
        public string FriendlyName { get; private set; }
        public string Description { get; private set; }
       // TODO:  Get the rest later

        private ItemSource() { }

        public static ItemSource Create()
        {
            return new ItemSource
            {
                
            };
        }
    }
}