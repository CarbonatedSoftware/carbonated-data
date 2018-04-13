using System;
using System.Runtime.Serialization;

namespace Carbonated.Data
{
    /// <summary>
    /// Exception that is thrown when errors occur during data loading.
    /// </summary>
    public class BindingException : Exception
    {
        public BindingException() { }
        public BindingException(string message) : base(message) { }
        public BindingException(string message, Exception innerException) : base(message, innerException) { }
        protected BindingException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }
}
