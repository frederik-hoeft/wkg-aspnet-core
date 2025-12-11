using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.DependencyInjection;
using Wkg.AspNetCore.TestAdapters.Initialization;

namespace Wkg.AspNetCore.TestAdapters;

/// <summary>
/// Represents a test that uses a database context.
/// </summary>
/// <typeparam name="TDbContext">The type of the database context.</typeparam>
/// <typeparam name="TInitializer">The <see cref="IAsyncDITestInitializer"/> implementation to be used for test initialization.</typeparam>
public abstract class DbContextTest<TDbContext, TInitializer> : DIAwareTestBase<TInitializer>
    where TDbContext : DbContext
    where TInitializer : IAsyncDITestInitializer
{
    /// <summary>
    /// Executes the specified unit test against the database context.
    /// </summary>
    /// <param name="unitTestAction">The unit test to be executed.</param>
    /// <param name="cancellationToken">The cancellation token to observe.</param>
    /// <remarks>
    /// Any changes made to the database context will be rolled back after the unit test has been executed.
    /// </remarks>
    /// <returns>A task that represents the asynchronous operation.</returns>
    protected Task UsingDbContextAsync(Action<TDbContext> unitTestAction, CancellationToken cancellationToken = default) => UsingServiceProviderAsync(async (serviceProvider, ct) =>
    {
        ArgumentNullException.ThrowIfNull(unitTestAction);
        TDbContext dbContext = serviceProvider.GetRequiredService<TDbContext>();
        await using IDbContextTransaction transaction = await dbContext.Database.BeginTransactionAsync(ct);
        try
        {
            unitTestAction.Invoke(dbContext);
        }
        finally
        {
            await transaction.RollbackAsync(ct);
        }
    }, cancellationToken);

    /// <summary>
    /// Executes the specified unit test asynchronously against the database context.
    /// </summary>
    /// <param name="unitTestTask">The unit test to be executed asynchronously.</param>
    /// <remarks>
    /// Any changes made to the database context will be rolled back after the unit test has been executed.
    /// </remarks>
    /// <returns>A task that represents the asynchronous operation.</returns>
    [Obsolete(DeprecationNotice.USE_CANCELLATION_TOKEN_OVERLOAD)]
    protected Task UsingDbContextAsync(Func<TDbContext, Task> unitTestTask) => UsingServiceProviderAsync(async serviceProvider =>
    {
        TDbContext dbContext = serviceProvider.GetRequiredService<TDbContext>();
        await using IDbContextTransaction transaction = await dbContext.Database.BeginTransactionAsync();
        try
        {
            await unitTestTask.Invoke(dbContext);
        }
        finally
        {
            await transaction.RollbackAsync();
        }
    });

    /// <summary>
    /// Executes the specified unit test asynchronously against the database context.
    /// </summary>
    /// <param name="unitTestTask">The unit test to be executed asynchronously.</param>
    /// <param name="cancellationToken">The cancellation token to observe.</param>
    /// <remarks>
    /// Any changes made to the database context will be rolled back after the unit test has been executed.
    /// </remarks>
    /// <returns>A task that represents the asynchronous operation.</returns>
    protected Task UsingDbContextAsync(Func<TDbContext, CancellationToken, Task> unitTestTask, CancellationToken cancellationToken = default) => UsingServiceProviderAsync(async (serviceProvider, ct) =>
    {
        TDbContext dbContext = serviceProvider.GetRequiredService<TDbContext>();
        await using IDbContextTransaction transaction = await dbContext.Database.BeginTransactionAsync(ct);
        try
        {
            await unitTestTask.Invoke(dbContext, ct);
        }
        finally
        {
            await transaction.RollbackAsync(ct);
        }
    }, cancellationToken);

    /// <summary>
    /// Executes the specified unit test asynchronously against the database context and scoped service provider.
    /// </summary>
    /// <param name="unitTestTask">The unit test to be executed asynchronously.</param>
    /// <param name="cancellationToken">The cancellation token to observe.</param>
    /// <remarks>
    /// Any changes made to the database context will be rolled back after the unit test has been executed.
    /// </remarks>
    /// <returns>A task that represents the asynchronous operation.</returns>
    protected Task UsingDbContextAsync(Func<TDbContext, IServiceProvider, CancellationToken, Task> unitTestTask, CancellationToken cancellationToken = default) => UsingServiceProviderAsync(async (serviceProvider, ct) =>
    {
        TDbContext dbContext = serviceProvider.GetRequiredService<TDbContext>();
        await using IDbContextTransaction transaction = await dbContext.Database.BeginTransactionAsync(ct);
        try
        {
            await unitTestTask.Invoke(dbContext, serviceProvider, ct);
        }
        finally
        {
            await transaction.RollbackAsync(ct);
        }
    }, cancellationToken);
}