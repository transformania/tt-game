using System;

namespace TT.Domain.Exceptions.RPClassifiedAds
{
    public sealed class RPClassifiedAdLimitException : RPClassifiedAdException
    {
        public RPClassifiedAdLimitException()
        {
        }

        public RPClassifiedAdLimitException(string message) : base(message)
        {
        }

        public RPClassifiedAdLimitException(string format, params object[] args) : base(format, args)
        {
        }

        public RPClassifiedAdLimitException(string message, Exception inner) : base(message, inner)
        {
        }
    }
}
