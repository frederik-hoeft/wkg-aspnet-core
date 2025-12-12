using System.Diagnostics.CodeAnalysis;
using Wkg.AspNetCore.Abstractions.Controllers;

namespace Wkg.AspNetCore.Exceptions;

/// <summary>
/// Represents an <see cref="Exception"/> that is (re-)thrown when an unhandled error was intercepted by an <see cref="WkgControllerBase"/> instance.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="ApiProxyException"/> class.
/// </remarks>
/// <param name="innerException">The inner exception that caused this exception to be thrown.</param>
[SuppressMessage("Design", "CA1032:Implement standard exception constructors", Justification = "This exception is only intended to be used as a wrapper for inner exceptions. It has no other state or behavior.")]
public sealed class ApiProxyException(Exception innerException) : Exception(nameof(ApiProxyException), innerException)
{
    /// <summary>
    /// Gets the message that describes the current exception.
    /// </summary>
    public override string? StackTrace => InnerException!.StackTrace;
}