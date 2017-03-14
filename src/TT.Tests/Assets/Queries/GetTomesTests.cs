using FluentAssertions;
using NUnit.Framework;
using TT.Domain;
using TT.Domain.Assets.Queries;
using TT.Tests.Builders.Assets;
using TT.Tests.Builders.Item;

namespace TT.Tests.Assets.Queries
{
    [TestFixture]
    public class GetTomesTests : TestBase
    {
        [Test]
        public void Should_fetch_all_available_tomes()
        {
            new TomeBuilder().With(cr => cr.Id, 7)
                .With(cr => cr.Text, "First Tome")
                .With(cr => cr.BaseItem, new ItemSourceBuilder().With(cr => cr.Id, 195).BuildAndSave())
                .BuildAndSave();

            new TomeBuilder().With(cr => cr.Id, 13)
                .With(cr => cr.Text, "Second Tome")
                .With(cr => cr.BaseItem, new ItemSourceBuilder().With(cr => cr.Id, 196).BuildAndSave())
                .BuildAndSave();

            var cmd = new GetTomes();

            var tomes = DomainRegistry.Repository.Find(cmd);

            tomes.Should().HaveCount(2);
        }

        [Test]
        public void Should_return_empty_list_if_no_tomes_found()
        {
            var cmd = new GetTomes();

            var tomes = DomainRegistry.Repository.Find(cmd);

            tomes.Should().BeEmpty();
        }

    }
}