using System;

namespace TT.Domain.Exceptions.Identity
{
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
    }
}
