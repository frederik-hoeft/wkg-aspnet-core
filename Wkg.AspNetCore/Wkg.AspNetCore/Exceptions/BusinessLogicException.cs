namespace Wkg.AspNetCore.Exceptions;

/// <summary>
/// Represents an exception that occurred in user-defined business logic.
/// </summary>
public sealed class BusinessLogicException : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="BusinessLogicException"/> class.
    /// </summary>
    public BusinessLogicException() => Pass();

    /// <summary>
    /// Initializes a new instance of the <see cref="BusinessLogicException"/> class with a specified error message.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    public BusinessLogicException(string? message) : base(message) => Pass();

    /// <summary>
    /// Initializes a new instance of the <see cref="BusinessLogicException"/> class with a specified error message and a reference to the inner exception that is the cause of this exception.
    /// </summary>
    /// <param name="message">The error message that explains the reason for the exception.</param>
    /// <param name="innerException">The exception that is the cause of the current exception, or a null reference if no inner exception is specified.</param>
    public BusinessLogicException(string message, Exception innerException) : base(message, innerException) => Pass();
}
