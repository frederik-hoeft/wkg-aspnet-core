using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.DependencyInjection;

namespace Wkg.AspNetCore.TestAdapters.Initialization;

/// <summary>
/// Provides a base class for asynchronous <see cref="IAsyncTestDatabaseLoader"/> implementations.
/// </summary>
/// <typeparam name="TSelf">The type of the implementing database loader.</typeparam>
/// <typeparam name="TDbContext">The type of the database context.</typeparam>
public abstract class AsyncTestDatabaseLoader<TSelf, TDbContext> : IAsyncTestDatabaseLoader
    where TSelf : AsyncTestDatabaseLoader<TSelf, TDbContext>, IAsyncTestDatabaseLoader<TDbContext>, new()
    where TDbContext : DbContext
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AsyncTestDatabaseLoader{TSelf, TDbContext}"/> class.
    /// </summary>
    protected AsyncTestDatabaseLoader()
    {
        if (this is not TSelf)
        {
            throw new InvalidOperationException($"The type {typeof(TSelf).Name} cannot be used as type parameter {nameof(TSelf)} in the generic type {nameof(AsyncTestDatabaseLoader<,>)} for derived type {GetType().Name}. " +
                $"There is no type parameter conversion from {GetType().Name} to {typeof(TSelf).Name}.");
        }
    }

    static async ValueTask IAsyncTestDatabaseLoader.InitializeDatabaseAsync(IServiceProvider serviceProvider, CancellationToken cancellationToken)
    {
        TSelf databaseLoader = new();
        await using TDbContext dbContext = serviceProvider.GetRequiredService<TDbContext>();
        await using IDbContextTransaction transaction = await dbContext.Database.BeginTransactionAsync(cancellationToken);
        await databaseLoader.InitializeDatabaseAsync(dbContext, cancellationToken);
        await transaction.CommitAsync(cancellationToken);
    }
}