using System;
using TT.Domain.Entities.Assets;
using TT.Tests.Builders.Identity;
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
            With(x => x.BaseItem, new ItemBuilder().BuildAndSave());
        }

        //public SetText(string text)
    }
}