using System.Collections.Generic;
using System.Threading.Tasks;
using Models;

namespace Interfaces
{
    public interface IGroupManager
    {
        List<Group> GetGroups(bool fullData = false);
        void UpdateGroup(Group gr);
        Task MergeGroups(Group from, Group to);
        void DeleteGroup(Group gr);
        void CreateGroup(Group gr);
    }
}