using NUnit.Framework;
using TT.Domain;
using TT.Domain.Exceptions;
using TT.Domain.Items.Queries;
using TT.Domain.Statics;
using TT.Tests.Builders.Item;

namespace TT.Tests.Items.Queries
{
    public class GetRandomRuneAtLevelTests : TestBase
    {
        [Test]
        public void should_get_a_valid_rune()
        {

            new ItemSourceBuilder()
                .With(i => i.Id, 100)
                .With(i => i.ItemType, PvPStatics.ItemType_Rune)
                .With(i => i.RuneLevel, 1)
                .BuildAndSave();

            new ItemSourceBuilder()
                .With(i => i.Id, 101)
                .With(i => i.ItemType, PvPStatics.ItemType_Rune)
                .With(i => i.RuneLevel, 1)
                .BuildAndSave();

            new ItemSourceBuilder()
                .With(i => i.Id, 102)
                .With(i => i.ItemType, PvPStatics.ItemType_Rune)
                .With(i => i.RuneLevel, 3)
                .BuildAndSave();

            new ItemSourceBuilder()
                .With(i => i.Id, 103)
                .With(i => i.ItemType, PvPStatics.ItemType_Rune)
                .With(i => i.RuneLevel, 4)
                .BuildAndSave();

            Assert.That(DomainRegistry.Repository.FindSingle(new GetRandomRuneAtLevel {RuneLevel = 1}),
                Is.EqualTo(100).Or.EqualTo(101));
        }

        [Test]
        public void throw_exception_if_RuneLevel_not_provided()
        {
            var query = new GetRandomRuneAtLevel {};
            Assert.That(() => Repository.FindSingle(query),
                Throws.TypeOf<DomainException>().With.Message.EqualTo("RuneLevel is required"));
        }

        [Test]
        public void throw_exception_if_RuneLevel_invalid()
        {
            var query = new GetRandomRuneAtLevel { RuneLevel = 12345};
            Assert.That(() => Repository.FindSingle(query),
                Throws.TypeOf<DomainException>().With.Message.EqualTo("RuneLevel '12345' is not a valid level."));
        }
    }
}
