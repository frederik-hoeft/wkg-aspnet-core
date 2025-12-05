namespace Wkg.AspNetCore.Authentication.Jwt.Internals;

internal sealed record ClaimValidationOptions(TimeSpan TimeToLive) : IClaimValidationOptions;