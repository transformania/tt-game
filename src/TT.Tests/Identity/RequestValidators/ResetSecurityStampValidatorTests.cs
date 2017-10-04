using MediatR;
using NSubstitute;
using NSubstitute.ClearExtensions;
using NSubstitute.Core;
using NUnit.Framework;
using System.Threading.Tasks;
using TT.Domain.Identity.CommandRequests;
using TT.Domain.Identity.Entities;
using TT.Domain.Identity.QueryRequestHandlers;
using TT.Domain.Identity.QueryRequests;
using TT.Domain.Identity.RequestValidators;
using TT.Domain.Statics;
using TT.Tests.Builders.Identity;

namespace TT.Tests.Identity.RequestValidators
{
    [Category("ResetSecurityStampValidator Tests")]
    public class ResetSecurityStampValidatorTests : TestBase
    {
        private ResetSecurityStampValidator validator;
        private ResetSecurityStamp request;

        private IMediator mediatorMock;
        private User adminUser;
        private User normalUser;

        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            mediatorMock = Substitute.For<IMediator>();

            validator = new ResetSecurityStampValidator(mediatorMock);
        }

        public override void SetUp()
        {
            base.SetUp();

            // unsure if this test fixture should include the internal handlers
            // as the validator has no clue about *how* the property is validated.
            // on the other hand, there isn't much of a better way to test a validator
            // than to do a kind of sort of integration test
            Task<bool> ReturnCall<TRequest, TRequestHandler>(CallInfo call)
                where TRequest : IRequest<bool>
                where TRequestHandler : IAsyncRequestHandler<TRequest, bool>
            {
                if (call.Args().Length > 0 && call.Args()[0] is TRequest request)
                {
                    var handler = (TRequestHandler) System.Activator.CreateInstance(typeof(TRequestHandler), DataContext);
                    return handler.Handle(request);
                }
                return Task.FromResult(false);
            }

            mediatorMock.Send(Arg.Any<UserHasAnyRoleRequest>()).Returns(ReturnCall<UserHasAnyRoleRequest, UserHasRoleRequestHandler>);
            mediatorMock.Send(Arg.Any<IsValidUserRequest>()).Returns(ReturnCall<IsValidUserRequest, IsValidUserRequestHandler>);

            var roleBuilder = new RoleBuilder();
            var userBuidler = new UserBuilder();

            roleBuilder.With(r => r.Name, PvPStatics.Permissions_Admin);
            roleBuilder.AddUser(userBuidler.Build());

            roleBuilder.BuildAndSave();
            adminUser = userBuidler.BuildAndSave();

            normalUser = new UserBuilder().BuildAndSave();

            request = new ResetSecurityStamp();
        }

        [TearDown]
        public void TearDown()
        {
            mediatorMock.ClearSubstitute(ClearOptions.All);
        }

        [Test]
        public async Task AssertValidatorHasErrorWhenRoleIsNotAdmin()
        {
            request.UserNameId = normalUser.Id;
            var results = await validator.ValidateAsync(request);

            Assert.That(results.Errors, Has.Exactly(1).Items);
        }

        [Test]
        public async Task AssertValidatorHasErrorWithCorrectErrorMessageWhenRoleIsNotAdmin()
        {
            request.UserNameId = normalUser.Id;
            var results = await validator.ValidateAsync(request);

            Assert.That(results.Errors[0].ErrorMessage, Is.EqualTo("User not in proper role."));
        }

        [Test]
        public async Task AssertValidatorHasNoErrorsWhenRoleIsAdmin()
        {
            // assume this passes
            mediatorMock.Send(Arg.Any<IsValidUserRequest>()).Returns(true);

            request.UserNameId = adminUser.Id;
            var results = await validator.ValidateAsync(request);

            Assert.That(results.Errors, Has.Exactly(0).Items);
        }

        [Test]
        public async Task AssertValidatorHasErrorWhenTargetUserIsNotAValidUser()
        {
            // assume this passes
            mediatorMock.Send(Arg.Any<UserHasAnyRoleRequest>()).Returns(true);
            request.TargetUserNameId = "not a user id";
            var results = await validator.ValidateAsync(request);

            Assert.That(results.Errors, Has.Exactly(1).Items);
        }

        [Test]
        public async Task AssertValidatorHasErrorWithCorrectErrorMessageWhenTargetUserIsNotAValidUser()
        {
            // assume this passes
            mediatorMock.Send(Arg.Any<UserHasAnyRoleRequest>()).Returns(true);

            request.TargetUserNameId = "not a user id";
            var results = await validator.ValidateAsync(request);

            Assert.That(results.Errors[0].ErrorMessage, Is.EqualTo("The user id given does not match any user."));
        }

        [Test]
        public async Task AssertValidatorDoesNOTCheckIfTargetUserIsValidWhenRoleIsNotAdmin()
        {
            request.UserNameId = normalUser.Id;
            var results = await validator.ValidateAsync(request);

            await mediatorMock.DidNotReceive().Send(Arg.Any<IsValidUserRequest>());
        }
    }
}
