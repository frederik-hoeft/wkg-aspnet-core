using System.Diagnostics.CodeAnalysis;

namespace Wkg.AspNetCore.TestAdapters;

/// <summary>
/// Provides a base class for all unit tests.
/// </summary>
public abstract class TestBase
{
    [SuppressMessage("Design", "CA1065:Do not raise exceptions in unexpected locations", Justification = "Guard clause to prevent undefined behavior due to version mismatches between frameworks.")]
    static TestBase()
    {
        if (!WkgAspNetCore.VersionInfo.VersionString.Equals(WkgAspNetCoreTestAdapters.VersionInfo.VersionString, StringComparison.Ordinal))
        {
            throw new InvalidOperationException(
                """
                Encountered a version mismatch between the Wkg.AspNetCore and Wkg.AspNetCore.TestAdapters frameworks.
                Please ensure that the unit test project is using the same version of the Wkg.AspNetCore.TestAdapters framework as the Wkg.AspNetCore framework that it is testing.
                """);
        }
    }
}
