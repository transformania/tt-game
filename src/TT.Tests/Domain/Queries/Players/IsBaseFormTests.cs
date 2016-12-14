

using FluentAssertions;
using NUnit.Framework;
using TT.Domain;
using TT.Domain.Queries.Players;
using TT.Tests.Builders.Form;

namespace TT.Tests.Domain.Queries.Players
{
    [TestFixture]
    public class IsBaseFormTests : TestBase
    {

        [SetUp]
        public void Init()
        {
            new FormSourceBuilder().With(u => u.dbName, "man_01")
                .With(u => u.FriendlyName, "Regular Guy")
                .BuildAndSave();

            new FormSourceBuilder().With(u => u.dbName, "woman_01")
                .With(u => u.FriendlyName, "Regular Girl")
                .BuildAndSave();

            new FormSourceBuilder().With(u => u.dbName, "derp")
                .With(u => u.FriendlyName, "Random form")
                .BuildAndSave();
        }

        [Test]
        public void should_return_true_if_form_is_valid_male()
        {
            var isValid = DomainRegistry.Repository.FindSingle(new IsBaseForm {form="man_01"});
            isValid.Should().Be(true);
        }

        [Test]
        public void should_return_true_if_form_is_valid_female()
        {
            var isValid = DomainRegistry.Repository.FindSingle(new IsBaseForm { form = "woman_01" });
            isValid.Should().Be(true);
        }

        [Test]
        public void should_return_false_if_form_is_invalid()
        {
            var isValid = DomainRegistry.Repository.FindSingle(new IsBaseForm { form = "derp" });
            isValid.Should().Be(false);
        }

        [Test]
        public void should_return_false_if_form_is_not_present()
        {
            var isValid = DomainRegistry.Repository.FindSingle(new IsBaseForm { form = "nonexistant!" });
            isValid.Should().Be(false);
        }

    }
}
