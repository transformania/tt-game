using System;

namespace TT.Domain.Exceptions.Identity
{
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
    }
}
