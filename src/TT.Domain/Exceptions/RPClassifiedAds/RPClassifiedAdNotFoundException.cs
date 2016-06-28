using System;
using System.Runtime.Serialization;

namespace TT.Domain.Exceptions.RPClassifiedAds
{
    [Serializable]
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

        private RPClassifiedAdNotFoundException(
            SerializationInfo info,
            StreamingContext context) : base(info, context)
        {
        }
    }
}
