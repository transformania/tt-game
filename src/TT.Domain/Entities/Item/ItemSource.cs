
namespace TT.Domain.Entities.Item
{
    public class ItemSource : Entity<int>
    {

        public string dbName { get; private set; }
        public string FriendlyName { get; private set; }
        public string Description { get; private set; }

        private ItemSource() { }

    }
}