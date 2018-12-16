using System;
using System.Linq;
using DAL;
using Interfaces;
using Models.NotDb;

namespace Services
{
    public class InfoService : IInfo
    {
        private readonly Ri2Context _dbContext;
        public InfoService(Ri2Context dbctx)
        {
            _dbContext = dbctx;

        }

        public DateTime LastUpdate
        {
            get
            {
                var info = _dbContext.Info.FirstOrDefault(i => i.Name == InfoKeys.LastUpdate);
                return info==null ? DateTime.MinValue : DateTime.Parse(info.Value);
            }
        }

        public int ProfilesCount => _dbContext.Profiles.Count();

        public string Version
        {
            get
            {
                var info = _dbContext.Info.FirstOrDefault(i => i.Name == InfoKeys.Version);
                return info == null ? "---" : info.Value;
            }
        }
    }
}