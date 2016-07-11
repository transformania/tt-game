using TT.Domain.Models;

namespace TT.Tests.Services
{
    public class TestData
    {
        public static T CreateRegularPlayer<T>(string membershipId = "100", string firstName = "Test", string lastName = "User", bool donator = true)
            where T : Player_VM, new()
        {
            return new T
            {
                MembershipId = membershipId, 
                FirstName = firstName, 
                LastName = lastName, 
                Nickname = donator ? "Wibble" : null, 
                DonatorLevel = donator ? 2 : 0
            };
        }

        public static T CreateStaffPlayer<T>()
            where T : Player_VM, new()
        {
            return new T { MembershipId = "69", FirstName = "Test", LastName = "User" };
        }
    }
}