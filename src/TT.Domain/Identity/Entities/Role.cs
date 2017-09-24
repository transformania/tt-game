using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TT.Domain.Entities;

namespace TT.Domain.Identity.Entities
{
    public class Role : Entity
    {
        public ICollection<User> Users { get; private set; }
        public string Name { get; private set; }
    }
}
