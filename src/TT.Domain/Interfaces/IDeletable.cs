using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TT.Domain.Interfaces
{
    public interface IDeletable
    {
        bool Deleted { get; }

        void Delete();
    }
}
