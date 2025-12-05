using System.Collections.Frozen;

namespace Wkg.AspNetCore.Configuration.ManagerBindings;

internal sealed record ManagerBindingOptions(FrozenDictionary<Type, ManagerFactory> Map);
