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
            Assert.That(DomainRegistry.Repository.FindSingle(new IsBaseForm {formSourceId = 1}), Is.True);
        }

        [Test]
        public void should_return_true_if_form_is_valid_female()
        {
            Assert.That(DomainRegistry.Repository.FindSingle(new IsBaseForm {formSourceId = 2}), Is.True);
        }

        [Test]
        public void should_return_false_if_form_is_invalid()
        {
            Assert.That(DomainRegistry.Repository.FindSingle(new IsBaseForm {formSourceId = 3}), Is.False);
        }

        [Test]
        public void should_return_false_if_form_is_not_present()
        {
            Assert.That(DomainRegistry.Repository.FindSingle(new IsBaseForm {formSourceId = 999}), Is.False);
        }
    }
}
