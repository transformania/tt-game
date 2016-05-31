using System;
using System.Runtime.Serialization;

namespace TT.Domain.Exceptions.RPClassifiedAds
{
    public class RPClassifiedAdException : DomainException
    {
        public enum ExceptionType
        {
            AdLimit,
            NotOwner,
            NotAUser,
            InvalidInput,
            NoAdfound
        }

        public ExceptionType ExType { get; set; }

        public string UserFriendlyError { get; set; }
        public string UserFriendlySubError { get; set; }

        public RPClassifiedAdException() : base()
        {
        }

        public RPClassifiedAdException(string message) : base(message)
        {
        }

        public RPClassifiedAdException(string format, params string[] args) : base(string.Format(format, args))
        {
        }

        public RPClassifiedAdException(string message, Exception inner) : base(message, inner)
        {
        }

        protected RPClassifiedAdException(
            SerializationInfo info,
            StreamingContext context) : base(info, context)
        {
        }
    }
}
