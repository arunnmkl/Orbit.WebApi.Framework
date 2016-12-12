using System;

namespace Orbit.WebApi.Base.SqlSerializer
{
    [Flags]
    public enum ParameterFlags
    {
        Default,
        IdFieldsOnly,
        ExcludeIdentityFields
    }
}