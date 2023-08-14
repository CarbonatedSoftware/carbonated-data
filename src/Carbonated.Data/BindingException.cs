using System;
using System.Runtime.Serialization;

namespace Carbonated.Data;

/// <summary>
/// Exception that is thrown when errors occur during data loading.
/// </summary>
public class BindingException : Exception
{
    /// <summary>
    /// Constructs an empty binding exception.
    /// </summary>
    public BindingException() { }

    /// <summary>
    /// Constructs a binding exception with the specified message.
    /// </summary>
    /// <param name="message">The exception message.</param>
    public BindingException(string message) : base(message) { }

    /// <summary>
    /// Constructs a binding exception that wraps another exception, with the specified message.
    /// </summary>
    /// <param name="message">The exception message.</param>
    /// <param name="innerException">The wrapped exception.</param>
    public BindingException(string message, Exception innerException) : base(message, innerException) { }

    /// <summary>
    /// Serialization constructor.
    /// </summary>
    protected BindingException(SerializationInfo info, StreamingContext context) : base(info, context) { }
}
