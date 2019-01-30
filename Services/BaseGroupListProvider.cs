using System.Collections.Generic;
using System.Linq;
using DAL;
using Interfaces;
using Models;

namespace Services.Helpers
{
    public class BaseGroupListProvider : IGroupListProvider
    {
        public IEnumerable<Group> GetGroups()
        {
            using (var db = new Ri2Context())
            {
                return db.Groups.ToList();
            }
        }
    }
}