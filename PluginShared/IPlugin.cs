using System;

namespace Interfaces
{
    public interface IPlugin
    {
        Guid GUID { get; }
        string Name { get; }
        string Author { get; }
        int Version { get; }
        string Description { get; }
    }
}