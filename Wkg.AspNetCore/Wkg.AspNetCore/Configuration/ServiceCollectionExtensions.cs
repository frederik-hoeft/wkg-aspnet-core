using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Wkg.AspNetCore.Configuration;

/// <summary>
/// Extension methods for <see cref="IServiceCollection"/>.
/// </summary>
public static class ServiceCollectionExtensions
{
#pragma warning disable CA1034 // Nested types should not be visible
    // TODO: IntelliSense doesn't yet recognize C# 14 extension syntax
    extension(IServiceCollection services)
#pragma warning restore CA1034 // Nested types should not be visible
    {
        /// <summary>
        /// Configures the <see cref="IServiceCollection"/> using the specified <typeparamref name="TStartupScript"/>.
        /// </summary>
        /// <typeparam name="TStartupScript">The type of the startup script.</typeparam>
        /// <param name="configuration">
        /// The <see cref="IConfiguration"/>. 
        /// If no configuration is specified, creates a new configuration that expects
        /// the "appsettings.json" file to exist. Additionally, the configuration also
        /// reads from "appsettings.[my_asp_environment].json", should the file exist.
        /// </param>
        /// <returns>The <see cref="IServiceCollection"/> for fluent configuration.</returns>
        [Obsolete(DeprecationNotice.SYNCHRONOUS_STARTUP_SCRIPT_INTERFACE)]
        public IServiceCollection ConfigureUsing<TStartupScript>(IConfiguration? configuration = null) where TStartupScript : IStartupScript
        {
            string env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Development";

            configuration ??= new ConfigurationBuilder()
                .SetBasePath(AppContext.BaseDirectory)
                .AddJsonFile("appsettings.json", optional: false)
                .AddJsonFile($"appsettings.{env}.json", optional: true)
                .AddEnvironmentVariables()
                .Build();

            TStartupScript.ConfigureServices(services, configuration);
            return services;
        }

        /// <summary>
        /// Asynchronously configures the <see cref="IServiceCollection"/> using the specified <typeparamref name="TAsyncStartupScript"/>.
        /// </summary>
        /// <typeparam name="TAsyncStartupScript">The type of the async startup script.</typeparam>
        /// <param name="configuration">
        /// The <see cref="IConfiguration"/>.
        /// If no configuration is specified, creates a new configuration that expects
        /// the "appsettings.json" file to exist. Additionally, the configuration also
        /// reads from "appsettings.[my_asp_environment].json", should the file exist.
        /// </param>
        /// <param name="cancellationToken">The cancellation token to observe.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the <see cref="IConfiguration"/> that was used.</returns>
        public async ValueTask<IConfiguration> ConfigureUsingAsync<TAsyncStartupScript>(IConfiguration? configuration = null, CancellationToken cancellationToken = default) where TAsyncStartupScript : IAsyncStartupScript
        {
            string env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Development";
            configuration ??= new ConfigurationBuilder()
                .SetBasePath(AppContext.BaseDirectory)
                .AddJsonFile("appsettings.json", optional: false)
                .AddJsonFile($"appsettings.{env}.json", optional: true)
                .AddEnvironmentVariables()
                .Build();
            await TAsyncStartupScript.ConfigureServicesAsync(services, configuration, cancellationToken).ConfigureAwait(false);
            return configuration;
        }
    }
}