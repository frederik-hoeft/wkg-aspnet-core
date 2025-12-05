using Microsoft.AspNetCore.Mvc;
using System.Runtime.CompilerServices;
using Wkg.AspNetCore.Transactions.Continuations;
using Wkg.AspNetCore.Transactions.Delegates;

namespace Wkg.AspNetCore.Transactions;

internal partial class Transaction<TDbContext>
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public IActionResult RunReadOnly(ReadOnlyDatabaseRequestAction<TDbContext, IActionResult> action) =>
        RunReadOnly<IActionResult>(action);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public IActionResult Run(DatabaseRequestAction<TDbContext, IActionResult> action) =>
        Run<IActionResult>(action);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void RunReadOnly(ReadOnlyDatabaseRequestAction<TDbContext> action) => Run((dbContext, transaction) =>
    {
        action.Invoke(dbContext);
        return new DeferredTransactionState<VoidResult>(TransactionState.ReadOnly, default);
    });

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Run(DatabaseRequestAction<TDbContext> action) => Run((dbContext, transaction) =>
    {
        IDeferredTransactionState continuation = action.Invoke(dbContext, transaction);
        return new DeferredTransactionState<VoidResult>(continuation.NextState, default);
    });

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public TResult RunReadOnly<TResult>(ReadOnlyDatabaseRequestAction<TDbContext, TResult> action) => Run((dbContext, transaction) =>
    {
        TResult result = action.Invoke(dbContext);
        return new DeferredTransactionState<TResult>(TransactionState.ReadOnly, result);
    });

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public TResult Run<TResult>(DatabaseRequestAction<TDbContext, TResult> action) =>
        RunInScope(action);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Task<IActionResult> RunReadOnlyAsync(ReadOnlyDatabaseRequestTask<TDbContext, IActionResult> task) =>
        RunReadOnlyAsync<IActionResult>(task);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Task<IActionResult> RunReadOnlyAsync(ReadOnlyDatabaseRequestTaskWithCancellation<TDbContext, IActionResult> task, CancellationToken cancellationToken) =>
        RunReadOnlyAsync<IActionResult>(task, cancellationToken);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Task<IActionResult> RunAsync(DatabaseRequestTask<TDbContext, IActionResult> task) => 
        RunAsync<IActionResult>(task);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Task<IActionResult> RunAsync(DatabaseRequestTaskWithCancellation<TDbContext, IActionResult> task, CancellationToken cancellationToken) =>
        RunAsync<IActionResult>(task, cancellationToken);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Task RunReadOnlyAsync(ReadOnlyDatabaseRequestTask<TDbContext> task) => RunInScopeAsync<VoidResult>(async (dbContext, transaction, _) =>
    {
        await task.Invoke(dbContext);
        return new DeferredTransactionState<VoidResult>(TransactionState.ReadOnly, default);
    }, CancellationToken.None);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Task RunReadOnlyAsync(ReadOnlyDatabaseRequestTaskWithCancellation<TDbContext> task, CancellationToken cancellationToken) => RunInScopeAsync<VoidResult>(async (dbContext, transaction, ct) =>
    {
        await task.Invoke(dbContext, ct);
        return new DeferredTransactionState<VoidResult>(TransactionState.ReadOnly, default);
    }, cancellationToken);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Task RunAsync(DatabaseRequestTask<TDbContext> task) => RunInScopeAsync<VoidResult>(async (dbContext, transaction, _) =>
    {
        IDeferredTransactionState continuation = await task.Invoke(dbContext, transaction);
        return new DeferredTransactionState<VoidResult>(continuation.NextState, default);
    }, CancellationToken.None);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Task RunAsync(DatabaseRequestTaskWithCancellation<TDbContext> task, CancellationToken cancellationToken) => RunInScopeAsync<VoidResult>(async (dbContext, transaction, ct) =>
    {
        IDeferredTransactionState continuation = await task.Invoke(dbContext, transaction, ct);
        return new DeferredTransactionState<VoidResult>(continuation.NextState, default);
    }, cancellationToken);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Task<TResult> RunReadOnlyAsync<TResult>(ReadOnlyDatabaseRequestTask<TDbContext, TResult> task) => RunInScopeAsync<TResult>(async (dbContext, transaction, _) =>
    {
        TResult result = await task.Invoke(dbContext);
        return new DeferredTransactionState<TResult>(TransactionState.ReadOnly, result);
    }, CancellationToken.None);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Task<TResult> RunReadOnlyAsync<TResult>(ReadOnlyDatabaseRequestTaskWithCancellation<TDbContext, TResult> task, CancellationToken cancellationToken) => RunInScopeAsync<TResult>(async (dbContext, transaction, ct) =>
    {
        TResult result = await task.Invoke(dbContext, ct);
        return new DeferredTransactionState<TResult>(TransactionState.ReadOnly, result);
    }, cancellationToken);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Task<TResult> RunAsync<TResult>(DatabaseRequestTask<TDbContext, TResult> task) => RunInScopeAsync<TResult>(async (dbContext, transaction, _) =>
    {
        IDeferredTransactionState<TResult> continuation = await task.Invoke(dbContext, transaction);
        return new DeferredTransactionState<TResult>(continuation.NextState, continuation.Result);
    }, CancellationToken.None);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Task<TResult> RunAsync<TResult>(DatabaseRequestTaskWithCancellation<TDbContext, TResult> task, CancellationToken cancellationToken) =>
        RunInScopeAsync(task, cancellationToken);
}
