namespace Wkg.AspNetCore.TestAdapters;

internal static class DeprecationNotice
{
    public const string USE_CANCELLATION_TOKEN_OVERLOAD = "Use an overload with a CancellationToken parameter instead. This method does not support cancellation and will be removed in future versions.";
}
