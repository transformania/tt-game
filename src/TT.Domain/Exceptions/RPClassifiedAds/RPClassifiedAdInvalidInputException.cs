using System;
using System.Runtime.Serialization;

namespace TT.Domain.Exceptions.RPClassifiedAds
{
    [Serializable]
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

        private RPClassifiedAdInvalidInputException(
            SerializationInfo info,
            StreamingContext context) : base(info, context)
        {
        }
    }
}
