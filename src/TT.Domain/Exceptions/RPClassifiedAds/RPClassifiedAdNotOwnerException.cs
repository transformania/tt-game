using System;

namespace TT.Domain.Exceptions.RPClassifiedAds
{
    public sealed class RPClassifiedAdNotOwnerException : RPClassifiedAdException
    {
        public RPClassifiedAdNotOwnerException()
        {
        }

        public RPClassifiedAdNotOwnerException(string message) : base(message)
        {
        }

        public RPClassifiedAdNotOwnerException(string format, params object[] args) : base(format, args)
        {
        }

        public RPClassifiedAdNotOwnerException(string message, Exception inner) : base(message, inner)
        {
        }
    }
}
