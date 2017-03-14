using System.Linq;
using FluentAssertions;
using NUnit.Framework;
using TT.Domain;
using TT.Domain.Players.Queries;
using TT.Tests.Builders.Form;

namespace TT.Tests.Players.Queries
{
    [TestFixture]
    public class GetBaseFormsTests : TestBase
    {
        [Test]
        public void should_return_base_forms()
        {

            new FormSourceBuilder().With(u => u.dbName, "man_01")
                .With(u => u.FriendlyName, "Regular Guy")
                .BuildAndSave();

            new FormSourceBuilder().With(u => u.dbName, "woman_01")
               .With(u => u.FriendlyName, "Regular Girl")
               .BuildAndSave();

            new FormSourceBuilder().With(u => u.dbName, "tempest")
               .With(u => u.FriendlyName, "Is A Big Fat Troll")
               .BuildAndSave();


            var forms = DomainRegistry.Repository.Find(new GetBaseForms());
            forms.Count().Should().Be(2);
            forms.ElementAt(0).FriendlyName.Should().Be("Regular Guy");
            forms.ElementAt(1).FriendlyName.Should().Be("Regular Girl");

        }
    }
}
