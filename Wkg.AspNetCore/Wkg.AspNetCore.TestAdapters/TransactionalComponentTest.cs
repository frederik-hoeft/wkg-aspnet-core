using Microsoft.EntityFrameworkCore;
using Wkg.AspNetCore.TestAdapters.Initialization;
using Wkg.AspNetCore.TestAdapters.Initialization.Extensions;

namespace Wkg.AspNetCore.TestAdapters;

/// <summary>
/// Represents the base class for tests of ASP.NET Core components that require transactional database operations.
/// </summary>
/// <remarks>
/// This class builds on <see cref="TransactionalTest{TDbContext, TInitializer}"/> and therefore relies on scoped
/// <see cref="Wkg.AspNetCore.Transactions.ITransaction{TDbContext}"/> instances that are bound to the lifetime of the
/// outermost service provider scope created by <see cref="DIAwareTestBase{TInitializer}"/>. All component interactions within
/// a single test method share the same ambient transaction.
/// </remarks>
/// <typeparam name="TComponent">The component under test that performs transactional database operations. Must be activatable using DI.</typeparam>
/// <typeparam name="TDbContext">The type of the database context.</typeparam>
/// <typeparam name="TInitializer">The <see cref="IAsyncDITestInitializer"/> implementation to be used for test initialization.</typeparam>
public abstract class TransactionalComponentTest<TComponent, TDbContext, TInitializer> : TransactionalTest<TDbContext, TInitializer>
    where TComponent : class
    where TDbContext : DbContext
    where TInitializer : IAsyncDITestInitializer
{
    /// <summary>
    /// Initializes the specified component before each test execution.
    /// </summary>
    /// <param name="component">The component to be initialized.</param>
    /// <param name="serviceProvider">The scoped service provider used to resolve additional dependencies.</param>
    /// <param name="cancellationToken">The cancellation token to observe.</param>
    /// <returns>A task that represents the asynchronous initialization operation.</returns>
    protected virtual ValueTask InitializeComponentAsync(TComponent component, IServiceProvider serviceProvider, CancellationToken cancellationToken) =>
        ValueTask.CompletedTask;

    /// <summary>
    /// Executes the specified unit test against a component, the scoped service provider, and a database context within a transactional scope.
    /// </summary>
    /// <param name="unitTestTask">The unit test delegate to execute. The delegate receives the component instance, the scoped service provider, the transactional database context, and a cancellation token.</param>
    /// <param name="cancellationToken">The cancellation token to observe.</param>
    /// <returns>A task that represents the asynchronous test execution.</returns>
    protected Task UsingComponentAsync(Func<TComponent, IServiceProvider, TDbContext, CancellationToken, Task> unitTestTask, CancellationToken cancellationToken = default) => 
        UsingTransactionAsync(async (dbContext, serviceProvider, ct) =>
    {
        TComponent component = serviceProvider.Activate<TComponent>();
        await InitializeComponentAsync(component, serviceProvider, ct);
        await unitTestTask.Invoke(component, serviceProvider, dbContext, ct);
    }, cancellationToken);

    /// <summary>
    /// Executes the specified unit test against a component and the scoped service provider within a transactional scope.
    /// </summary>
    /// <param name="unitTestTask">The unit test delegate to execute. The delegate receives the component instance, the scoped service provider, and a cancellation token.</param>
    /// <param name="cancellationToken">The cancellation token to observe.</param>
    /// <returns>A task that represents the asynchronous test execution.</returns>
    protected Task UsingComponentAsync(Func<TComponent, IServiceProvider, CancellationToken, Task> unitTestTask, CancellationToken cancellationToken = default) => 
        UsingTransactionAsync(async (_, serviceProvider, ct) =>
    {
        TComponent component = serviceProvider.Activate<TComponent>();
        await InitializeComponentAsync(component, serviceProvider, ct);
        await unitTestTask.Invoke(component, serviceProvider, ct);
    }, cancellationToken);

    /// <summary>
    /// Executes the specified unit test against a component within a transactional scope.
    /// </summary>
    /// <param name="unitTestTask">The unit test delegate to execute. The delegate receives the component instance and a cancellation token.</param>
    /// <param name="cancellationToken">The cancellation token to observe.</param>
    /// <returns>A task that represents the asynchronous test execution.</returns>
    protected Task UsingComponentAsync(Func<TComponent, CancellationToken, Task> unitTestTask, CancellationToken cancellationToken = default) =>
        UsingTransactionAsync(async (_, serviceProvider, ct) =>
        {
            TComponent component = serviceProvider.Activate<TComponent>();
            await InitializeComponentAsync(component, serviceProvider, ct);
            await unitTestTask.Invoke(component, ct);
        }, cancellationToken);
}
