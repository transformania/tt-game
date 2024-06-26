using System;

namespace TT.Domain.Exceptions.RPClassifiedAds
{
    public sealed class RPClassifiedAdNotFoundException : RPClassifiedAdException
    {
        public RPClassifiedAdNotFoundException()
        {
        }

        public RPClassifiedAdNotFoundException(string message) : base(message)
        {
        }

        public RPClassifiedAdNotFoundException(string format, params object[] args) : base(format, args)
        {
        }

        public RPClassifiedAdNotFoundException(string message, Exception inner) : base(message, inner)
        {
        }
    }
}
