using System;
using System.Runtime.Serialization;

namespace TT.Domain.Exceptions.Identity
{
    [Serializable]
    public sealed class UserNotFoundException : UserException
    {
        public UserNotFoundException()
        {
        }

        public UserNotFoundException(string message) : base(message)
        {
        }

        public UserNotFoundException(string format, params object[] args) : base(format, args)
        {
        }

        public UserNotFoundException(string message, Exception inner) : base(message, inner)
        {
        }

        private UserNotFoundException(
            SerializationInfo info,
            StreamingContext context) : base(info, context)
        {
        }
    }
}
