using Microsoft.AspNetCore.Builder;
using System.Runtime.CompilerServices;

namespace Wkg.AspNetCore.Configuration;

/// <summary>
/// Extension methods for <see cref="WebApplicationBuilder"/>.
/// </summary>
public static class WebApplicationBuilderExtensions
{
#pragma warning disable CA1034 // Nested types should not be visible
    // TODO: IntelliSense doesn't yet recognize C# 14 extension syntax
    extension(WebApplicationBuilder builder)
#pragma warning restore CA1034 // Nested types should not be visible
    {
        /// <summary>
        /// Configures the <see cref="WebApplicationBuilder.Services"/> of the provided <see cref="WebApplicationBuilder"/> using the specified <typeparamref name="TStartupScript"/>.
        /// </summary>
        /// <typeparam name="TStartupScript">The type of the startup script.</typeparam>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [Obsolete(DeprecationNotice.SYNCHRONOUS_STARTUP_SCRIPT_INTERFACE)]
        public void ConfigureServicesUsing<TStartupScript>() where TStartupScript : IStartupScript
        {
            ArgumentNullException.ThrowIfNull(builder);
            TStartupScript.ConfigureServices(builder.Services, builder.Configuration);
        }

        /// <summary>
        /// Configures the provided <see cref="WebApplicationBuilder"/> and the resulting <see cref="WebApplication"/> using the specified <typeparamref name="TStartupScript"/>.
        /// </summary>
        /// <typeparam name="TStartupScript">The type of the startup script.</typeparam>
        /// <returns>The configured <see cref="WebApplication"/>.</returns>
        [Obsolete(DeprecationNotice.SYNCHRONOUS_STARTUP_SCRIPT_INTERFACE)]
        public WebApplication BuildUsing<TStartupScript>() where TStartupScript : IStartupScript
        {
            ArgumentNullException.ThrowIfNull(builder);
            builder.ConfigureServicesUsing<TStartupScript>();
            WebApplication app = builder.Build();
            TStartupScript.Configure(app);
            return app;
        }

        /// <summary>
        /// Asynchronously configures the <see cref="WebApplicationBuilder.Services"/> of the provided <see cref="WebApplicationBuilder"/> using the specified <typeparamref name="TAsyncStartupScript"/>.
        /// </summary>
        /// <typeparam name="TAsyncStartupScript">The type of the async startup script.</typeparam>
        /// <returns>A task that represents the asynchronous operation.</returns>
        public async ValueTask ConfigureServicesUsingAsync<TAsyncStartupScript>() where TAsyncStartupScript : IAsyncStartupScript
        {
            ArgumentNullException.ThrowIfNull(builder);
            await TAsyncStartupScript.ConfigureServicesAsync(builder.Services, builder.Configuration).ConfigureAwait(false);
        }

        /// <summary>
        /// Asynchronously configures the provided <see cref="WebApplicationBuilder"/> and the resulting <see cref="WebApplication"/> using the specified <typeparamref name="TAsyncStartupScript"/>.
        /// </summary>
        /// <typeparam name="TAsyncStartupScript">The type of the async startup script.</typeparam>
        /// <returns>A task that represents the asynchronous operation. The task result contains the configured <see cref="WebApplication"/>.</returns>

        public async ValueTask<WebApplication> BuildUsingAsync<TAsyncStartupScript>() where TAsyncStartupScript : IAsyncStartupScript
        {
            ArgumentNullException.ThrowIfNull(builder);
            await builder.ConfigureServicesUsingAsync<TAsyncStartupScript>().ConfigureAwait(false);
            WebApplication app = builder.Build();
            await TAsyncStartupScript.ConfigureAsync(app).ConfigureAwait(false);
            return app;
        }
    }
}