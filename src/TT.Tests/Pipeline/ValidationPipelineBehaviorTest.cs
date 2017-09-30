using FluentValidation;
using FluentValidation.Results;
using MediatR;
using NSubstitute;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
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
        }

        public override void SetUp()
        {
            base.SetUp();

            nextMock = Substitute.For<RequestHandlerDelegate<int>>();
            validatorMock = Substitute.ForPartsOf<AbstractValidator<IRequestTest>>();
            validationPipelineBehavior = new ValidationPipelineBehavior<IRequestTest, int>(principalAccessorMock, validatorMock);
            principalAccessorMock.UserNameId.Returns(userNameId);
        }

        [TearDown]
        public void TearDown()
        {
            requestMock.RuleSets.Returns(RuleSets.None);
        }

        [Test]
        public async Task AssertRuleSetIsPassed()
        {
            bool rulesetRan = false;
            requestMock.RuleSets.Returns(RuleSets.Admin);

            validatorMock.RuleSet(RuleSets.Admin.ToString(), () =>
            {
                validatorMock.RuleFor(m => m).Custom((property, context) =>
                {
                    rulesetRan = true;
                });
            });

            await validationPipelineBehavior.Handle(requestMock, nextMock);
            Assert.IsTrue(rulesetRan);
        }

        [Test]
        public async Task AssertIdIsPassed()
        {
            string userNameId = null;

            idValidatorMock.RuleFor(m => m.UserNameId).Custom((property, context) =>
            {
                userNameId = property;
            });
            validatorMock.Include(idValidatorMock);

            await validationPipelineBehavior.Handle(requestMock, nextMock);
            Assert.AreEqual(this.userNameId, userNameId);
        }

        [Test]
        public void AssertValidationExceptionIsThrownWhenNotValid()
        {
            validatorMock.RuleFor(m => m).Custom((property, context) =>
            {
                context.AddFailure("Property", "Error");
            });

            Assert.ThrowsAsync<ValidationException>(() => validationPipelineBehavior.Handle(requestMock, nextMock));
        }

        [Test]
        public async Task AssertValidateAsyncIsUsed()
        {
            await validationPipelineBehavior.Handle(requestMock, nextMock);

            await validatorMock.Received().ValidateAsync(Arg.Any<ValidationContext<IRequestTest>>());
        }

        [Test]
        public async Task AssertNextIsCalledWhenSuccessful()
        {
            await validationPipelineBehavior.Handle(requestMock, nextMock);

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
                await validationPipelineBehavior.Handle(requestMock, nextMock);
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
