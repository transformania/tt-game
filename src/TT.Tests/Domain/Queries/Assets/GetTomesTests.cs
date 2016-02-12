using FluentAssertions;
using NUnit.Framework;
using TT.Domain;
using TT.Domain.Commands.Assets;
using TT.Domain.Queries.Assets;
using TT.Tests.Builders.Assets;
using TT.Tests.Builders.Item;

namespace TT.Tests.Domain.Queries
{
    [TestFixture]
    public class GetTomesTests : TestBase
    {
        [Test]
        public void Should_fetch_all_available_tomes()
        {
            var item = new ItemBuilder().BuildAndSave();

            var cmd = new CreateTome { Text = "Tome1", BaseItemId = item.Id };
            var tome1 = Repository.Execute(cmd);

            var cmd1 = new CreateTome { Text = "Tome2", BaseItemId = item.Id };
            var tome2 = Repository.Execute(cmd);

            var cmd3 = new GetTomes();

            var tomes = DomainRegistry.Repository.Find(cmd3);

            tomes.Should().HaveCount(2);
        }

        [Test]
        public void Should_return_empty_list_if_no_tomes_found()
        {
            var cmd = new GetTomes();

            var tomes = DomainRegistry.Repository.Find(cmd);

            tomes.Should().BeEmpty();
        }

        [Test]
        public void Should_fetch_tome_by_id()
        {
            var builder = new TomeBuilder().With(cr => cr.Id, 7)
                .With(cr => cr.Text, "First Tome")
                .With(cr => cr.BaseItem, new ItemBuilder().With(cr => cr.Id, 195).BuildAndSave())
                .BuildAndSave();

            var getTomeCmd = new GetTome(7);

            var tome = DomainRegistry.Repository.Find(getTomeCmd);

            tome.Id.Should().Equals(7);
            tome.Text.Should().BeEquivalentTo("First Tome");
            tome.BaseItem.Id.Should().Equals(195);

        }
    }
}