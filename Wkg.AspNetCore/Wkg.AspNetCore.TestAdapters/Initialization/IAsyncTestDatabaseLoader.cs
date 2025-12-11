using Microsoft.EntityFrameworkCore;

namespace Wkg.AspNetCore.TestAdapters.Initialization;

/// <summary>
/// Represents asynchronous setup code that is executed before the first test of the first test class is run.
/// </summary>
public interface IAsyncTestDatabaseLoader
{
    internal static abstract ValueTask InitializeDatabaseAsync(IServiceProvider serviceProvider, CancellationToken cancellationToken);
}

/// <inheritdoc />
/// <typeparam name="TDbContext">The type of the database context.</typeparam>
public interface IAsyncTestDatabaseLoader<in TDbContext> : IAsyncTestDatabaseLoader
    where TDbContext : DbContext
{
    /// <summary>
    /// Asynchronously initializes the database by inserting the data that is required for the tests.
    /// </summary>
    /// <param name="dbContext">The database context to be used to interact with the database.</param>
    /// <param name="serviceProvider">The service provider to be used to resolve additional services.</param>
    /// <param name="cancellationToken">The cancellation token to observe.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    ValueTask InitializeDatabaseAsync(TDbContext dbContext, IServiceProvider serviceProvider, CancellationToken cancellationToken);
}