using NUnit.Framework;
using System.Threading.Tasks;
using TT.Domain.Identity.Entities;
using TT.Domain.Identity.QueryRequestHandlers;
using TT.Domain.Identity.QueryRequests;
using TT.Tests.Builders.Identity;

namespace TT.Tests.Identity.QueryRequestHandlers
{
    [Category("IsValidUserRequestHandler Tests")]
    public class IsValidUserRequestHandlerTests : TestBase
    {
        private User user;
        private IsValidUserRequestHandler requestHandler;
        private IsValidUserRequest request;

        public override void SetUp()
        {
            base.SetUp();

            user = new UserBuilder().BuildAndSave();
            requestHandler = new IsValidUserRequestHandler(DataContext);
            request = new IsValidUserRequest();
        }

        [Test]
        public async Task AssertRequestHandlerReturnsTrueWhenAUserExists()
        {
            request.UserNameId = user.Id;
            var result = await requestHandler.Handle(request);

            Assert.That(result, Is.True);
        }

        [Test]
        public async Task AssertRequestHandlerReturnsFalseWhenNoUserExists()
        {
            request.UserNameId = "not a valid id";
            var result = await requestHandler.Handle(request);

            Assert.That(result, Is.False);
        }
    }
}
