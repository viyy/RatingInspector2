using System;

namespace Interfaces
{
    public interface IInfo
    {
        DateTime LastUpdate { get; }
        int ProfilesCount { get; }
        string Version { get; }
    }
}