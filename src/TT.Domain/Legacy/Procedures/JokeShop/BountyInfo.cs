using TT.Domain.Models;

namespace TT.Domain.Legacy.Procedures.JokeShop
{
    public class BountyInfo
    {
        public string PlayerName;
        public DbStaticForm Form;
        public string Category;
        public int ExpiresTurn;
        public int CurrentReward;
    }
}
