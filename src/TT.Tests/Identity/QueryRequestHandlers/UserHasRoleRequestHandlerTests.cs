using NUnit.Framework;
using System.Threading.Tasks;
using TT.Domain.Identity.Entities;
using TT.Domain.Identity.QueryRequestHandlers;
using TT.Domain.Identity.QueryRequests;
using TT.Domain.Statics;
using TT.Tests.Builders.Identity;

namespace TT.Tests.Identity.QueryRequestHandlers
{
    [Category("UserHasRoleRequestHandler Tests")]
    public class UserHasRoleRequestHandlerTests : TestBase
    {
        private User user;
        private UserHasRoleRequestHandler requestHandler;
        private UserHasAnyRoleRequest request;

        public override void SetUp()
        {
            base.SetUp();

            var roleBuilder = new RoleBuilder();
            var userBuidler = new UserBuilder();

            roleBuilder.With(r => r.Name, PvPStatics.Permissions_Admin);
            roleBuilder.AddUser(userBuidler.Build());

            roleBuilder.BuildAndSave();
            user = userBuidler.BuildAndSave();

            requestHandler = new UserHasRoleRequestHandler(DataContext);
            request = new UserHasAnyRoleRequest();
        }

        [Test]
        public async Task AssertRequestHandlerReturnsTrueWhenUserSharesCommonRoll()
        {
            request.UserNameId = user.Id;
            request.Role = new[] { PvPStatics.Permissions_Artist, PvPStatics.Permissions_Chaoslord, PvPStatics.Permissions_Admin };
            var result = await requestHandler.Handle(request);

            Assert.That(result, Is.True);
        }

        [Test]
        public async Task AssertRequestHandlerReturnsFalseWhenUserSharesNoCommonRole()
        {
            request.UserNameId = user.Id;
            request.Role = new[] { PvPStatics.Permissions_Artist, PvPStatics.Permissions_Chaoslord };
            var result = await requestHandler.Handle(request);

            Assert.That(result, Is.False);
        }
    }
}
