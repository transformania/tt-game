using System;
using System.Runtime.Serialization;

namespace TT.Domain.Exceptions.RPClassifiedAds
{
    [Serializable]
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

        private RPClassifiedAdLimitException(
            SerializationInfo info,
            StreamingContext context) : base(info, context)
        {
        }
    }
}
