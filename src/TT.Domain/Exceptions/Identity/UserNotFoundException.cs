using System;

namespace TT.Domain.Exceptions.Identity
{
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
    }
}
