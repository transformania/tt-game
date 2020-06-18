using System.Threading;
using FluentValidation;
using MediatR;
using NSubstitute;
using NSubstitute.ClearExtensions;
using NUnit.Framework;
using System.Threading.Tasks;
using TT.Domain.RequestInterfaces;
using TT.Domain.Services;
using TT.Domain.Validation;

namespace TT.Tests.Pipeline
{
    [Category("ValidationPipelineBehavior Tests")]
    public class ValidationPipelineBehaviorTest : TestBase
    {
        private IPrincipalAccessor principalAccessorMock;
        private AbstractValidator<IRequestTest> validatorMock;
        private AbstractValidator<IRequestWithUserNameId> idValidatorMock;
        private ValidationPipelineBehavior<IRequestTest, int> validationPipelineBehavior;
        private IRequestTest requestMock;
        private RequestHandlerDelegate<int> nextMock;

        private string userNameId = "1";

        [OneTimeSetUp]
        public void TestFixtureSetup()
        {
            principalAccessorMock = Substitute.For<IPrincipalAccessor>();
            idValidatorMock = Substitute.ForPartsOf<AbstractValidator<IRequestWithUserNameId>>();

            requestMock = Substitute.For<IRequestTest>();
            nextMock = Substitute.For<RequestHandlerDelegate<int>>();

            principalAccessorMock.UserNameId.Returns(userNameId);
        }

        public override void SetUp()
        {
            base.SetUp();

            validatorMock = Substitute.ForPartsOf<AbstractValidator<IRequestTest>>();
            validationPipelineBehavior = new ValidationPipelineBehavior<IRequestTest, int>(principalAccessorMock, validatorMock);
        }

        [TearDown]
        public void TearDown()
        {
            idValidatorMock.ClearSubstitute(ClearOptions.All);
            nextMock.ClearSubstitute(ClearOptions.All);
            requestMock.ClearSubstitute(ClearOptions.All);
        }

        [Test]
        public async Task AssertRuleSetIsPassed()
        {
            var rulesetRan = false;
            requestMock.RuleSets.Returns(RuleSets.Admin);

            validatorMock.RuleSet(RuleSets.Admin.ToString(), () =>
            {
                validatorMock.RuleFor(m => m).Custom((property, context) =>
                {
                    rulesetRan = true;
                });
            });

            var token = new CancellationToken();
            await validationPipelineBehavior.Handle(requestMock, token, nextMock);
            Assert.That(rulesetRan, Is.True);
        }

        [Test]
        public async Task AssertIdIsPassed()
        {
            string capturedUserNameId = null;

            idValidatorMock.RuleFor(m => m.UserNameId).Custom((property, context) =>
            {
                capturedUserNameId = property;
            });
            validatorMock.Include(idValidatorMock);

            var token = new CancellationToken();
            await validationPipelineBehavior.Handle(requestMock, token, nextMock);
            Assert.That(userNameId, Is.EqualTo(capturedUserNameId));
        }

        [Test]
        public void AssertValidationExceptionIsThrownWhenNotValid()
        {
            validatorMock.RuleFor(m => m).Custom((property, context) =>
            {
                context.AddFailure("Property", "Error");
            });

            async Task Delegate()
            {
                var token = new CancellationToken();
                await validationPipelineBehavior.Handle(requestMock, token, nextMock);
            };

            Assert.That(Delegate, Throws.TypeOf<ValidationException>());
        }

        [Test]
        public async Task AssertValidateAsyncIsUsed()
        {
            var token = new CancellationToken();

            await validationPipelineBehavior.Handle(requestMock, token, nextMock);

            await validatorMock.Received().ValidateAsync(Arg.Any<ValidationContext<IRequestTest>>(), token);
        }

        [Test]
        public async Task AssertNextIsCalledWhenSuccessful()
        {
            var token = new CancellationToken();

            await validationPipelineBehavior.Handle(requestMock, token, nextMock);

            await nextMock.Received().Invoke();
        }

        [Test]
        public async Task AssertNextIsNotCalledWhenNotSuccessful()
        {
            validatorMock.RuleFor(m => m).Custom((property, context) =>
            {
                context.AddFailure("Property", "Error");
            });

            try
            {
                var token = new CancellationToken();
                await validationPipelineBehavior.Handle(requestMock, token, nextMock);
            }
            catch
            {
                // we aren't testing if this method throws
            }

            await nextMock.DidNotReceive().Invoke();
        }
    }

    public interface IRequestTest : IRequest, IRequestWithUserNameId, IRequestWithRuleSet
    {
    }
}
