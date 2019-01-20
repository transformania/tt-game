using FluentAssertions;
using NUnit.Framework;
using TT.Domain;
using TT.Domain.Players.Queries;
using TT.Tests.Builders.Form;

namespace TT.Tests.Players.Queries
{
    [TestFixture]
    public class IsBaseFormTests : TestBase
    {

        [SetUp]
        public void Init()
        {
            new FormSourceBuilder()
                .With(u => u.Id, 1)
                .With(u => u.FriendlyName, "Regular Guy")
                .BuildAndSave();

            new FormSourceBuilder()
                .With(u => u.Id, 2)
                .With(u => u.FriendlyName, "Regular Girl")
                .BuildAndSave();

            new FormSourceBuilder()
                .With(u => u.Id, 3)
                .With(u => u.FriendlyName, "Random form")
                .BuildAndSave();
        }

        [Test]
        public void should_return_true_if_form_is_valid_male()
        {
            var isValid = DomainRegistry.Repository.FindSingle(new IsBaseForm { formSourceId = 1 });
            isValid.Should().Be(true);
        }

        [Test]
        public void should_return_true_if_form_is_valid_female()
        {
            var isValid = DomainRegistry.Repository.FindSingle(new IsBaseForm { formSourceId = 2 });
            isValid.Should().Be(true);
        }

        [Test]
        public void should_return_false_if_form_is_invalid()
        {
            var isValid = DomainRegistry.Repository.FindSingle(new IsBaseForm { formSourceId = 3 });
            isValid.Should().Be(false);
        }

        [Test]
        public void should_return_false_if_form_is_not_present()
        {
            var isValid = DomainRegistry.Repository.FindSingle(new IsBaseForm { formSourceId = 999 });
            isValid.Should().Be(false);
        }

    }
}
