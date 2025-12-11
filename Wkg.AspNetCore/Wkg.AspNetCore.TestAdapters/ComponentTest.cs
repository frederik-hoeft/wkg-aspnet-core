using Wkg.AspNetCore.TestAdapters.Initialization;
using Wkg.AspNetCore.TestAdapters.Initialization.Extensions;

namespace Wkg.AspNetCore.TestAdapters;

/// <summary>
/// Represents the base class for tests of ASP.NET Core components that require dependency injection services.
/// </summary>
/// <typeparam name="TComponent">The type of the component to be tested. Must be activatable using DI.</typeparam>
/// <typeparam name="TInitializer">The <see cref="IAsyncDITestInitializer"/> implementation to be used for test initialization.</typeparam>
public abstract class ComponentTest<TComponent, TInitializer> : DIAwareTestBase<TInitializer>
    where TComponent : class
    where TInitializer : IAsyncDITestInitializer
{
    /// <summary>
    /// Executes the specified unit test against the component in a dedicated DI scope.
    /// </summary>
    /// <param name="unitTestAction">The unit test action to be executed.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    protected static Task UsingComponentAsync(Action<TComponent> unitTestAction) => UsingServiceProviderAsync(serviceProvider =>
    {
        TComponent component = serviceProvider.Activate<TComponent>();
        unitTestAction.Invoke(component);
    });

    /// <summary>
    /// Executes the specified unit test asynchronously against the component in a dedicated DI scope.
    /// </summary>
    /// <param name="unitTestTask">The unit test task to be executed asynchronously.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    [Obsolete(DeprecationNotice.USE_CANCELLATION_TOKEN_OVERLOAD)]
    protected Task UsingComponentAsync(Func<TComponent, Task> unitTestTask) => UsingServiceProviderAsync(async serviceProvider =>
    {
        TComponent component = serviceProvider.Activate<TComponent>();
        await unitTestTask.Invoke(component);
    });

    /// <summary>
    /// Executes the specified unit test asynchronously against the component in a dedicated DI scope.
    /// </summary>
    /// <param name="unitTestTask">The unit test task to be executed asynchronously.</param>
    /// <param name="cancellationToken">The cancellation token to observe.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    protected static Task UsingComponentAsync(Func<TComponent, CancellationToken, Task> unitTestTask, CancellationToken cancellationToken = default) => UsingServiceProviderAsync(async (serviceProvider, ct) =>
    {
        TComponent component = serviceProvider.Activate<TComponent>();
        await unitTestTask.Invoke(component, ct);
    }, cancellationToken);

    /// <summary>
    /// Executes the specified unit test asynchronously against the component and service provider in a dedicated DI scope.
    /// </summary>
    /// <param name="unitTestTask">The unit test task to be executed asynchronously.</param>
    /// <param name="cancellationToken">The cancellation token to observe.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    protected static Task UsingComponentAsync(Func<TComponent, IServiceProvider, CancellationToken, Task> unitTestTask, CancellationToken cancellationToken = default) => UsingServiceProviderAsync(async (serviceProvider, ct) =>
    {
        TComponent component = serviceProvider.Activate<TComponent>();
        await unitTestTask.Invoke(component, serviceProvider, ct);
    }, cancellationToken);
}
