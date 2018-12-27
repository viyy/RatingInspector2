using System;

namespace Interfaces
{
    public interface IInfo
    {
        DateTime LastUpdate { get; }
        int ProfilesCount { get; }
        int RcfCount { get; }
        int FideCount { get; }
        string Version { get; }
    }
}