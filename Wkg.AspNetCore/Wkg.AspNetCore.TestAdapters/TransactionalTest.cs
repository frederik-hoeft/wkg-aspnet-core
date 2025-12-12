using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Wkg.AspNetCore.TestAdapters.Initialization;
using Wkg.AspNetCore.Transactions;

namespace Wkg.AspNetCore.TestAdapters;

/// <summary>
/// Represents the base class for tests that execute code within a transactional scope for a specific <see cref="DbContext"/>.
/// </summary>
/// <remarks>
/// The transactional helpers provided by this class rely on the scoped <see cref="ITransaction{TDbContext}"/> infrastructure
/// that is bound to the lifetime of the outermost dependency injection scope created by <see cref="DIAwareTestBase{TInitializer}"/>.
/// All transactional operations performed by a single test method therefore participate in the same ambient transaction.
/// </remarks>
/// <typeparam name="TDbContext">The type of the database context used by the transaction service.</typeparam>
/// <typeparam name="TInitializer">The <see cref="IAsyncDITestInitializer"/> implementation to be used for test initialization.</typeparam>
public abstract class TransactionalTest<TDbContext, TInitializer> : DIAwareTestBase<TInitializer>
    where TDbContext : DbContext
    where TInitializer : IAsyncDITestInitializer
{
    /// <summary>
    /// Executes the specified unit test within a read-only transactional scope, providing access to the database context and scoped service provider.
    /// </summary>
    /// <param name="unitTestTask">The unit test delegate to execute. The delegate receives the database context, the scoped service provider, and a cancellation token.</param>
    /// <param name="cancellationToken">The cancellation token to observe.</param>
    /// <returns>A task that represents the asynchronous test execution.</returns>
    protected Task UsingTransactionAsync(Func<TDbContext, IServiceProvider, CancellationToken, Task> unitTestTask, CancellationToken cancellationToken = default) => UsingServiceProviderAsync(async (serviceProvider, ct) =>
    {
        ITransactionService<TDbContext> transactionContext = serviceProvider.GetRequiredService<ITransactionService<TDbContext>>();
        await transactionContext.Scoped.RunReadOnlyAsync((dbContext, ct2) => unitTestTask.Invoke(dbContext, serviceProvider, ct2), ct);
    }, cancellationToken);

    /// <summary>
    /// Executes the specified unit test within a read-only transactional scope, providing access only to the database context.
    /// </summary>
    /// <param name="unitTestTask">The unit test delegate to execute. The delegate receives the database context and a cancellation token.</param>
    /// <param name="cancellationToken">The cancellation token to observe.</param>
    /// <returns>A task that represents the asynchronous test execution.</returns>
    protected Task UsingTransactionAsync(Func<TDbContext, CancellationToken, Task> unitTestTask, CancellationToken cancellationToken = default) => UsingServiceProviderAsync(async (serviceProvider, ct) =>
    {
        ITransactionService<TDbContext> transactionContext = serviceProvider.GetRequiredService<ITransactionService<TDbContext>>();
        await transactionContext.Scoped.RunReadOnlyAsync((dbContext, ct2) => unitTestTask.Invoke(dbContext, ct2), ct);
    }, cancellationToken);
}
