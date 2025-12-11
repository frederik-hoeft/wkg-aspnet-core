using Microsoft.Extensions.DependencyInjection;
using Wkg.AspNetCore.TestAdapters.Initialization;
using Wkg.Threading;

namespace Wkg.AspNetCore.TestAdapters;

/// <summary>
/// Provides a base class for all unit tests that require dependency injection.
/// </summary>
/// <typeparam name="TInitializer">The <see cref="IAsyncDITestInitializer"/> implementation to be used for test initialization.</typeparam>
public abstract class DIAwareTestBase<TInitializer> : TestBase where TInitializer : IAsyncDITestInitializer
{
    private static readonly AsyncLock s_asyncLock = new();
    private static volatile ServiceProvider? s_serviceProvider;

    private protected static async ValueTask<ServiceProvider> GetServiceProviderAsync(CancellationToken cancellationToken)
    {
        ServiceProvider? serviceProvider = s_serviceProvider;
        serviceProvider ??= await s_asyncLock.RunTaskAsync(async ct =>
        {
            // double-check locking
            ServiceProvider? sp = s_serviceProvider;
            if (sp is null)
            {
                // initialize DI
                ServiceCollection services = new();
                await TInitializer.ConfigureAsync(services, ct).ConfigureAwait(false);
                sp = services.BuildServiceProvider();
                await TInitializer.InitializeAsync(sp, ct).ConfigureAwait(false);
                s_serviceProvider = sp;
            }
            return sp;
        }, cancellationToken).ConfigureAwait(false);
        return serviceProvider;
    }

    /// <summary>
    /// Executes the specified unit test in a dedicated DI scope.
    /// </summary>
    /// <param name="unitTestAction">The unit test to be executed.</param>
    /// <param name="cancellationToken">The cancellation token to observe.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    protected static async Task UsingServiceProviderAsync(Action<IServiceProvider> unitTestAction, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(unitTestAction);
        ServiceProvider serviceProvider = await GetServiceProviderAsync(cancellationToken).ConfigureAwait(false);
        IServiceScopeFactory scopeFactory = serviceProvider.GetRequiredService<IServiceScopeFactory>();
        await using AsyncServiceScope scope = scopeFactory.CreateAsyncScope();
        unitTestAction.Invoke(scope.ServiceProvider);
    }

    /// <summary>
    /// Executes the specified unit test asynchronously in a dedicated DI scope.
    /// </summary>
    /// <param name="unitTestTask">The unit test to be executed asynchronously.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    [Obsolete(DeprecationNotice.USE_CANCELLATION_TOKEN_OVERLOAD)]
    protected async Task UsingServiceProviderAsync(Func<IServiceProvider, Task> unitTestTask)
    {
        ArgumentNullException.ThrowIfNull(unitTestTask);
        ServiceProvider serviceProvider = await GetServiceProviderAsync(CancellationToken.None).ConfigureAwait(false);
        IServiceScopeFactory scopeFactory = serviceProvider.GetRequiredService<IServiceScopeFactory>();
        await using AsyncServiceScope scope = scopeFactory.CreateAsyncScope();
        await unitTestTask.Invoke(scope.ServiceProvider);
    }

    /// <summary>
    /// Executes the specified unit test asynchronously in a dedicated DI scope.
    /// </summary>
    /// <param name="unitTestTask">The unit test to be executed asynchronously.</param>
    /// <param name="cancellationToken">The cancellation token to observe.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    protected static async Task UsingServiceProviderAsync(Func<IServiceProvider, CancellationToken, Task> unitTestTask, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(unitTestTask);
        ServiceProvider serviceProvider = await GetServiceProviderAsync(cancellationToken).ConfigureAwait(false);
        IServiceScopeFactory scopeFactory = serviceProvider.GetRequiredService<IServiceScopeFactory>();
        await using AsyncServiceScope scope = scopeFactory.CreateAsyncScope();
        await unitTestTask.Invoke(scope.ServiceProvider, cancellationToken);
    }
}
