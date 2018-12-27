using System;
using System.Linq;
using Common;
using DAL;
using Interfaces;

namespace Services
{
    public class InfoService : IInfo
    {
        public DateTime LastUpdate
        {
            get
            {
                using (var dbContext = new Ri2Context())
                {
                    var info = dbContext.Info.FirstOrDefault(i => i.Name == InfoKeys.LastUpdate);
                    return info == null ? DateTime.MinValue : DateTime.Parse(info.Value);
                }
            }
        }

        public int ProfilesCount
        {
            get
            {
                using (var db = new Ri2Context())
                {
                    return db.Profiles.Count();
                }
            }
        }

        public int RcfCount
        {
            get
            {
                using (var db = new Ri2Context())
                {
                    return db.RcfProfiles.Count();
                }
            }
        }

        public int FideCount
        {
            get
            {
                using (var db = new Ri2Context())
                {
                    return db.FideProfiles.Count();
                }
            }
        }

        public string Version
        {
            get
            {
                using (var db = new Ri2Context())
                {
                    var info = db.Info.FirstOrDefault(i => i.Name == InfoKeys.Version);
                    return info == null ? "---" : info.Value;
                }
            }
        }
    }
}