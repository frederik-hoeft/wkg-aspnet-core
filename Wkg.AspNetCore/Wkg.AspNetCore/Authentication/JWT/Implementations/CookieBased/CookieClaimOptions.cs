using Wkg.AspNetCore.Authentication.Jwt.Internals;

namespace Wkg.AspNetCore.Authentication.Jwt.Implementations.CookieBased;

internal sealed record CookieClaimOptions(bool SecureOnly, ClaimValidationOptions ValidationOptions);