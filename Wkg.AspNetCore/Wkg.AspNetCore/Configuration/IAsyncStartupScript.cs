using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Wkg.AspNetCore.Configuration;

/// <summary>
/// Represents an asynchronous startup script that configures a web application.
/// Serves as an integration test-ready replacement for the <see cref="IStartup"/> implementation.
/// </summary>
public interface IAsyncStartupScript
{
    /// <summary>
    /// Asynchronously configures the specified <paramref name="app"/>.
    /// </summary>
    /// <param name="app">The <see cref="WebApplication"/> to configure.</param>
    /// <param name="cancellationToken">The cancellation token to observe.</param>
    /// <returns>A <see cref="ValueTask"/> representing the asynchronous operation.</returns>
    static abstract ValueTask ConfigureAsync(WebApplication app, CancellationToken cancellationToken);

    /// <summary>
    /// Asynchronously configures the specified <paramref name="services"/> for dependency injection.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to configure.</param>
    /// <param name="configuration">The <see cref="IConfiguration"/>.</param>
    /// <param name="cancellationToken">The cancellation token to observe.</param>
    /// <returns>A <see cref="ValueTask"/> representing the asynchronous operation.</returns>
    static abstract ValueTask ConfigureServicesAsync(IServiceCollection services, IConfiguration configuration, CancellationToken cancellationToken);
}