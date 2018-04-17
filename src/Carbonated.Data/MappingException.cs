using System;
using System.Runtime.Serialization;

namespace Carbonated.Data
{
    /// <summary>
    /// Exception that is thrown when errors occur during data mapping.
    /// </summary>
    public class MappingException : Exception
    {
        /// <summary>
        /// Constructs an empty mapping exception.
        /// </summary>
        public MappingException() { }

        /// <summary>
        /// Constructs a mapping exception with the specified message.
        /// </summary>
        /// <param name="message">The exception message.</param>
        public MappingException(string message) : base(message) { }

        /// <summary>
        /// Constructs a mapping exception that wraps another exception, with the specified message.
        /// </summary>
        /// <param name="message">The exception message.</param>
        /// <param name="innerException">The wrapped exception.</param>
        public MappingException(string message, Exception innerException) : base(message, innerException) { }

        /// <summary>
        /// Serialization constructor.
        /// </summary>
        protected MappingException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }
}
