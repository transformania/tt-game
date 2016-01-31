using TT.Domain.Models;

namespace TT.Tests.Services
{
    public class TestData
    {
        public static Player_VM CreateRegularPlayer(string membershipId = "100", string firstName = "Test", string lastName = "User", bool donator = true)
        {
            return new Player_VM
            {
                MembershipId = membershipId, 
                FirstName = firstName, 
                LastName = lastName, 
                Nickname = donator ? "Wibble" : null, 
                DonatorLevel = donator ? 2 : 0
            };
        }

        public static Player_VM CreateStaffPlayer()
        {
            return new Player_VM { MembershipId = "69", FirstName = "Test", LastName = "User" };
        }
    }
}