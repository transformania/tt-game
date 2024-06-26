using System;

namespace TT.Domain.Exceptions.RPClassifiedAds
{
    public sealed class RPClassifiedAdInvalidInputException : RPClassifiedAdException
    {
        public RPClassifiedAdInvalidInputException()
        {
        }

        public RPClassifiedAdInvalidInputException(string message) : base(message)
        {
        }

        public RPClassifiedAdInvalidInputException(string format, params object[] args) : base(format, args)
        {
        }

        public RPClassifiedAdInvalidInputException(string message, Exception inner) : base(message, inner)
        {
        }
    }
}
