using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Wkg.AspNetCore.TestAdapters.Initialization;

namespace Wkg.AspNetCore.TestAdapters;

/// <summary>
/// Represents the base class for tests of ASP.NET Core controller components that participate in transactional database operations.
/// </summary>
/// <remarks>
/// As a specialized derivative of <see cref="TransactionalComponentTest{TComponent, TDbContext, TInitializer}"/>, this class relies
/// on scoped <see cref="Wkg.AspNetCore.Transactions.ITransaction{TDbContext}"/> instances that are tied to the lifetime of the
/// outermost dependency injection scope created by <see cref="DIAwareTestBase{TInitializer}"/>. All database work performed by a
/// controller within a single test method therefore participates in the same ambient transaction.
/// </remarks>
/// <typeparam name="TController">The type of the controller under test.</typeparam>
/// <typeparam name="TDbContext">The type of the database context used for transactions.</typeparam>
/// <typeparam name="TInitializer">The <see cref="IAsyncDITestInitializer"/> implementation to be used for test initialization.</typeparam>
public abstract class TransactionalControllerTest<TController, TDbContext, TInitializer> : TransactionalComponentTest<TController, TDbContext, TInitializer>
    where TController : ControllerBase
    where TDbContext : DbContext
    where TInitializer : IAsyncDITestInitializer
{
    /// <summary>
    /// Initializes the controller before each test execution by attaching an <see cref="HttpContext"/> and configuring the controller context.
    /// </summary>
    /// <param name="component">The controller instance to initialize.</param>
    /// <param name="serviceProvider">The scoped service provider used to resolve required services.</param>
    /// <param name="cancellationToken">The cancellation token to observe.</param>
    /// <returns>A task that represents the asynchronous initialization operation.</returns>
    protected override ValueTask InitializeComponentAsync(TController component, IServiceProvider serviceProvider, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(component);
        IHttpContextAccessor httpContextAccessor = serviceProvider.GetRequiredService<IHttpContextAccessor>();
        httpContextAccessor.HttpContext ??= new DefaultHttpContext
        {
            RequestServices = serviceProvider,
        };
        component.ControllerContext = new ControllerContext
        {
            HttpContext = httpContextAccessor.HttpContext
        };
        return base.InitializeComponentAsync(component, serviceProvider, cancellationToken);
    }

    /// <summary>
    /// Executes the specified unit test against a controller, the scoped service provider, and a transactional database context after validating the specified request model.
    /// </summary>
    /// <typeparam name="TRequest">The type of the request model to validate.</typeparam>
    /// <param name="request">The request model instance that will be validated using <see cref="ControllerBase.TryValidateModel(object?)"/>.</param>
    /// <param name="unitTestTask">The unit test delegate to execute. The delegate receives the controller instance, the scoped service provider, the transactional database context, and a cancellation token.</param>
    /// <param name="cancellationToken">The cancellation token to observe.</param>
    /// <returns>A task that represents the asynchronous test execution.</returns>
    protected Task UsingComponentAsync<TRequest>(TRequest request, Func<TController, IServiceProvider, TDbContext, CancellationToken, Task> unitTestTask, CancellationToken cancellationToken = default) =>
        UsingComponentAsync(async (controller, serviceProvider, dbContext, ct) =>
        {
            ArgumentNullException.ThrowIfNull(request);
            controller.TryValidateModel(request);
            await unitTestTask.Invoke(controller, serviceProvider, dbContext,  ct);
        }, cancellationToken);

    /// <summary>
    /// Executes the specified unit test against a controller and the scoped service provider after validating the specified request model.
    /// </summary>
    /// <typeparam name="TRequest">The type of the request model to validate.</typeparam>
    /// <param name="request">The request model instance that will be validated using <see cref="ControllerBase.TryValidateModel(object?)"/>.</param>
    /// <param name="unitTestTask">The unit test delegate to execute. The delegate receives the controller instance, the scoped service provider, and a cancellation token.</param>
    /// <param name="cancellationToken">The cancellation token to observe.</param>
    /// <returns>A task that represents the asynchronous test execution.</returns>
    protected Task UsingComponentAsync<TRequest>(TRequest request, Func<TController, IServiceProvider, CancellationToken, Task> unitTestTask, CancellationToken cancellationToken = default) =>
        UsingComponentAsync(async (controller, serviceProvider, ct) =>
        {
            ArgumentNullException.ThrowIfNull(request);
            controller.TryValidateModel(request);
            await unitTestTask.Invoke(controller, serviceProvider, ct);
        }, cancellationToken);

    /// <summary>
    /// Executes the specified unit test against a controller after validating the specified request model.
    /// </summary>
    /// <typeparam name="TRequest">The type of the request model to validate.</typeparam>
    /// <param name="request">The request model instance that will be validated using <see cref="ControllerBase.TryValidateModel(object?)"/>.</param>
    /// <param name="unitTestTask">The unit test delegate to execute. The delegate receives the controller instance and a cancellation token.</param>
    /// <param name="cancellationToken">The cancellation token to observe.</param>
    /// <returns>A task that represents the asynchronous test execution.</returns>
    protected Task UsingComponentAsync<TRequest>(TRequest request, Func<TController, CancellationToken, Task> unitTestTask, CancellationToken cancellationToken = default) =>
        UsingComponentAsync(async (controller, ct) =>
        {
            ArgumentNullException.ThrowIfNull(request);
            controller.TryValidateModel(request);
            await unitTestTask.Invoke(controller, ct);
        }, cancellationToken);
}
