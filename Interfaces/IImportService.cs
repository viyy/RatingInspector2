using System.Collections.Generic;
using System.Threading.Tasks;
using Common;
using Models;

namespace Interfaces
{
    public interface IImportService
    {
        IEnumerable<Group> GetGroups();
        string GetFilters();
        Task ImportAsync(IEnumerable<int> ids, Group group, ProfileType profileType);
    }
}