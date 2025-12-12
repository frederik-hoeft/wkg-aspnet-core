namespace Wkg.AspNetCore.Transactions;

/// <summary>
/// Represents an exception that is thrown when a concurrency violation is detected.
/// </summary>
/// <param name="message">The message that describes the error.</param>
public sealed class ConcurrencyViolationException(string? message) : Exception(message);
