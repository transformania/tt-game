namespace TT.Domain.Entities.Identity
{
    public class User : Entity<string>
    {
        public string UserName { get; private set; }
        public string Email { get; private set; }

        private User() { }
    }
}