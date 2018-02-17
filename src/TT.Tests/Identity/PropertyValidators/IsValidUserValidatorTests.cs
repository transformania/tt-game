using FluentValidation;
using FluentValidation.Internal;
using FluentValidation.Validators;
using MediatR;
using NSubstitute;
using NSubstitute.ClearExtensions;
using NUnit.Framework;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using TT.Domain.Identity.PropertyValidators;
using TT.Domain.Identity.QueryRequests;

namespace TT.Tests.Identity.PropertyValidators
{
    [Category("IsValidUserValidator Tests")]
    public class IsValidUserValidatorTests : TestBase
    {
        private IsValidUserValidator isValidUserValidator;
        private IMediator mediatorMock;
        private PropertyValidatorContext propertyValidatorContext;
        private PropertyValidatorContext invalidPropertyValidatorContext;

        private IsValidUserRequest request;

        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            var userNameId = "TestUser";

            request = new IsValidUserRequest()
            {
                UserNameId = userNameId
            };

            mediatorMock = Substitute.For<IMediator>();
            isValidUserValidator = new IsValidUserValidator(mediatorMock);

            var parentContext = new ValidationContext(null);
            var rule = new PropertyRule(null, x => null, null, null, typeof(string), null);
            propertyValidatorContext = new PropertyValidatorContext(parentContext, rule, "UserNameId", userNameId);
            invalidPropertyValidatorContext = new PropertyValidatorContext(parentContext, rule, "Integer", 0);
        }

        public override void SetUp()
        {
            base.SetUp();
        }

        [TearDown]
        public void TearDown()
        {
            mediatorMock.ClearSubstitute(ClearOptions.All);
        }

        [Test]
        public async Task AssertMediatorSendIsCalledAfterValidateAsync()
        {
            await isValidUserValidator.ValidateAsync(propertyValidatorContext, default(CancellationToken));
            await mediatorMock.ReceivedWithAnyArgs().Send(request, default(CancellationToken));
        }

        [Test]
        public async Task AssertMediatorSendIsCalledWithTheCorrectRequestType()
        {
            await isValidUserValidator.ValidateAsync(propertyValidatorContext, default(CancellationToken));
            await mediatorMock.Received().Send(Arg.Any<IsValidUserRequest>(), Arg.Any<CancellationToken>());
        }

        [Test]
        public async Task AssertMediatorSendIsCalledWithTheCorrectRequestAndProperties()
        {
            Expression<Predicate<IsValidUserRequest>> expression = (other) =>
                request.UserNameId == other.UserNameId;

            await isValidUserValidator.ValidateAsync(propertyValidatorContext, default(CancellationToken));
            await mediatorMock.Received().Send(Arg.Is(expression), Arg.Any<CancellationToken>());
        }

        [Test]
        public async Task AssertPropertyIsInvalidIfMediatorSendReturnsFalse()
        {
            mediatorMock.Send(Arg.Any<IsValidUserRequest>(), Arg.Any<CancellationToken>()).Returns(false);

            var result = await isValidUserValidator.ValidateAsync(propertyValidatorContext, default(CancellationToken));
            Assert.That(result, Has.Exactly(1).Items);
        }

        [Test]
        public async Task AssertPropertyIsInvalidIfValueTypeIsInvalid()
        {
            var result = await isValidUserValidator.ValidateAsync(invalidPropertyValidatorContext, default(CancellationToken));
            Assert.That(result, Has.Exactly(1).Items);
        }

        [Test]
        public async Task AssertErrorCodeIsCorrectWhenPropertyIsInvalid()
        {
            var result = await isValidUserValidator.ValidateAsync(invalidPropertyValidatorContext, default(CancellationToken));
            Assert.That(result.Single().ErrorCode, Is.EqualTo(nameof(IsValidUserValidator)));
        }
    }
}
