using System.Collections.Generic;
using TT.Domain.Entities;

namespace TT.Domain.Identity.Entities
{
    public class Role : Entity
    {
        public ICollection<User> Users { get; private set; }
        public string Name { get; private set; }

        private Role() { }
    }
}
