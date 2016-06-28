using System;
using System.Runtime.Serialization;

namespace TT.Domain.Exceptions.Identity
{
    [Serializable]
    public class UserException : DomainException
    {
        public UserException()
        {
        }

        public UserException(string message) : base(message)
        {
        }

        public UserException(string format, params object[] args) : base(format, args)
        {
        }

        public UserException(string message, Exception inner) : base(message, inner)
        {
        }

        protected UserException(
            SerializationInfo info,
            StreamingContext context) : base(info, context)
        {
        }
    }
}
