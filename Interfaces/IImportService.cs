﻿using System.Collections.Generic;
using System.Threading.Tasks;
using Common;
using Models;

namespace Interfaces
{
    public interface IImportService : IGroupListProvider
    {
        string GetFilters();
        IEnumerable<int> LoadFromFile(string path);
        Task ImportAsync(IEnumerable<int> ids, Group group, ProfileType profileType);
    }
}