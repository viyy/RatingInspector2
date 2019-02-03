using System.Collections.Generic;
using Models;

namespace Interfaces
{
    public interface IGroupListProvider
    {
        IEnumerable<Group> GetGroups();
    }
}