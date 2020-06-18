using NUnit.Framework;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TT.Domain.Identity.CommandHandlers;
using TT.Domain.Identity.CommandRequests;
using TT.Domain.Identity.Entities;
using TT.Tests.Builders.Identity;

namespace TT.Tests.Identity.Commands
{
    [Category("ResetSecurityStamp Tests")]
    public class ResetSecurityStampTest : TestBase
    {
        private User testUser;
        private UserSecurityStamp testUserSecurityStamp;

        private ResetSecurityStamp resetSecurityStamp;
        private ResetSecurityStampHandler handler;

        public override void SetUp()
        {
            base.SetUp();

            var UserStampBuilder = new UserSecurityStampBuilder();
            var UserBuilder = new UserBuilder();

            UserStampBuilder.AssignToUser(UserBuilder.Build());

            testUser = UserBuilder.BuildAndSave();
            testUserSecurityStamp = UserStampBuilder.BuildAndSave();

            resetSecurityStamp = new ResetSecurityStamp() { TargetUserNameId = testUser.Id };
            handler = new ResetSecurityStampHandler(DataContext);
        }

        [Test]
        public async Task AssertStampResets()
        {
            var previousStampValue = testUserSecurityStamp.SecurityStamp;
            var token = new CancellationToken();
            await handler.Handle(resetSecurityStamp, token);
            var currentStampValue = DataContext.AsQueryable<UserSecurityStamp>().First().SecurityStamp;

            Assert.That(previousStampValue, Is.Not.EqualTo(currentStampValue));
        }
    }
}
