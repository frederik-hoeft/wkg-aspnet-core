using Microsoft.Extensions.DependencyInjection;

namespace Wkg.AspNetCore.TestAdapters.Initialization;

/// <summary>
/// Represents asynchronous dependency injection setup code that is executed before the first test of the first test class requiring DI is executed.
/// </summary>
public interface IAsyncDITestInitializer
{
    /// <summary>
    /// Configures the specified <paramref name="services"/> for dependency injection.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to configure.</param>
    /// <param name="cancellationToken">The cancellation token to observe.</param>
    static abstract ValueTask ConfigureAsync(IServiceCollection services, CancellationToken cancellationToken);

    /// <summary>
    /// Performs additional asynchronous initialization steps after the DI container has been built.
    /// </summary>
    /// <param name="serviceProvider">The built <see cref="IServiceProvider"/>.</param>
    /// <param name="cancellationToken">The cancellation token to observe.</param>
    static abstract ValueTask InitializeAsync(IServiceProvider serviceProvider, CancellationToken cancellationToken);
}