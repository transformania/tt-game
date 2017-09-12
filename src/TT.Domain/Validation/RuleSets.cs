using System;

namespace TT.Domain.Validation
{
    [Flags]
    public enum RuleSets
    {
        None        = 0,
        Default     = 1 << 0,   // 1
        Admin       = 1 << 1,   // 2
        Moderator   = 1 << 2,   // 4
        All         = ~0        // Default | Admin | Moderator
    }
}
