using System.Linq;
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

            new FormSourceBuilder()
                .With(u => u.FriendlyName, "Regular Guy")
                .BuildAndSave();

            new FormSourceBuilder()
               .With(u => u.FriendlyName, "Regular Girl")
               .BuildAndSave();

            new FormSourceBuilder()
               .With(u => u.FriendlyName, "Is A Big Fat Troll")
               .BuildAndSave();


            var forms = DomainRegistry.Repository.Find(new GetBaseForms()).ToList();
            Assert.That(forms, Has.Exactly(2).Items);
            Assert.That(forms.ElementAt(0).FriendlyName, Is.EqualTo("Regular Guy"));
            Assert.That(forms.ElementAt(1).FriendlyName, Is.EqualTo("Regular Girl"));

        }
    }
}
