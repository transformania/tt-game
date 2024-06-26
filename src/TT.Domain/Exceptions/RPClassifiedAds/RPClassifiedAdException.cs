using System;

namespace TT.Domain.Exceptions.RPClassifiedAds
{
    public class RPClassifiedAdException : DomainException
    {
        public RPClassifiedAdException()
        {
        }

        public RPClassifiedAdException(string message) : base(message)
        {
        }

        public RPClassifiedAdException(string format, params object[] args) : base(format, args)
        {
        }

        public RPClassifiedAdException(string message, Exception inner) : base(message, inner)
        {
        }
    }
}