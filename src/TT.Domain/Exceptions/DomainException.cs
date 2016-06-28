using System;
using System.Runtime.Serialization;

namespace TT.Domain//.Exceptions this needs to be refactored by someone with ReSharper
{
    [Serializable]
    public class DomainException : Exception
    {
        public string UserFriendlyError { get; set; }
        public string UserFriendlySubError { get; set; }

        public DomainException()
        {
        }

        public DomainException(string message) : base(message)
        {
        }

        public DomainException(string format, params object[] args) : base(string.Format(format, args))
        {
        }

        public DomainException(string message, Exception inner) : base(message, inner)
        {
        }

        protected DomainException(
            SerializationInfo info,
            StreamingContext context) : base(info, context)
        {
        }
    }
}