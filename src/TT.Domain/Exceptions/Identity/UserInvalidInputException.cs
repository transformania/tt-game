using System;
using System.Runtime.Serialization;

namespace TT.Domain.Exceptions.Identity
{
    [Serializable]
    public sealed class UserInvalidInputException : UserException
    {
        public UserInvalidInputException()
        {
        }

        public UserInvalidInputException(string message) : base(message)
        {
        }

        public UserInvalidInputException(string format, params object[] args) : base(format, args)
        {
        }

        public UserInvalidInputException(string message, Exception inner) : base(message, inner)
        {
        }

        private UserInvalidInputException(
            SerializationInfo info,
            StreamingContext context) : base(info, context)
        {
        }
    }
}
