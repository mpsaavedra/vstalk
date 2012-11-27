// Guids.cs
// MUST match guids.h
using System;

namespace Microsoft.VSTalk
{
    static class GuidList
    {
        public const string guidVSTalkPkgString = "4a5fca27-f846-43e3-9411-a1f3792ce8cd";
        public const string guidVSTalkCmdSetString = "fae8afb6-4715-424e-9619-b2b0ca2a1b18";
        public const string guidToolWindowPersistanceString = "4aaf9d89-bf38-4ed9-b0b5-1f324bfbbdc9";

        public static readonly Guid guidVSTalkCmdSet = new Guid(guidVSTalkCmdSetString);
    };
}