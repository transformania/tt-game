using FluentAssertions;
using NUnit.Framework;
using TT.Domain;
using TT.Domain.Chat.Queries;
using TT.Tests.Builders.Chat;

namespace TT.Tests.Domain.Queries.Chat
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

            var rooms = DomainRegistry.Repository.Find(cmd);

            rooms.Should().HaveCount(2);
        }

        [Test]
        public void Should_return_empty_list_if_no_rooms_found()
        {
            var cmd = new GetChatRooms();

            var rooms = DomainRegistry.Repository.Find(cmd);

            rooms.Should().BeEmpty();
        }
    }
}