using System;
using System.Runtime.Serialization;

namespace Carbonated.Data
{
    /// <summary>
    /// Exception that is thrown when errors occur during data mapping.
    /// </summary>
    public class MappingException : Exception
    {
        public MappingException() { }
        public MappingException(string message) : base(message) { }
        public MappingException(string message, Exception innerException) : base(message, innerException) { }
        protected MappingException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }
}
