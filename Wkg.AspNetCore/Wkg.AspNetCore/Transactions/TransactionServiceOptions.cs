using System.Data;

namespace Wkg.AspNetCore.Transactions;

internal sealed record TransactionServiceOptions(IsolationLevel TransactionIsolationLevel);