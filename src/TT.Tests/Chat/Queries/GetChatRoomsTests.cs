using NUnit.Framework;
using TT.Domain;
using TT.Domain.Chat.Queries;
using TT.Tests.Builders.Chat;

namespace TT.Tests.Chat.Queries
{
    [TestFixture]
    public class GetChatRoomsTests : TestBase
    {
        [Test]
        public void Should_fetch_all_available_rooms()
        {
            new ChatRoomBuilder().With(cr => cr.Name, "Room1").BuildAndSave();
            new ChatRoomBuilder().With(cr => cr.Name, "Room2").BuildAndSave();

            var cmd = new GetChatRooms();

            Assert.That(DomainRegistry.Repository.Find(cmd), Has.Exactly(2).Items);
        }

        [Test]
        public void Should_return_empty_list_if_no_rooms_found()
        {
            var cmd = new GetChatRooms();

            Assert.That(DomainRegistry.Repository.Find(cmd), Is.Empty);
        }
    }
}