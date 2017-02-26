using TT.Domain.Assets.Entities;
using TT.Tests.Builders.Item;

namespace TT.Tests.Builders.Assets
{
    public class TomeBuilder : Builder<Tome, int>
    {
        public TomeBuilder()
        {
            Instance = Create();
            With(x => x.Id, 7);
            With(x => x.Text, "tome!");
            With(x => x.BaseItem, new ItemSourceBuilder().BuildAndSave());
        }

    }
}