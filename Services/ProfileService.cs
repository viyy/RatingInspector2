using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using DAL;
using Interfaces;
using Models;
using Services.Helpers;

namespace Services
{
    public class ProfileService : BaseGroupListProvider, IProfileManager
    {
        public IEnumerable<Profile> GetProfiles(string needle = null)
        {
            var res = new List<Profile>();
            using (var db = new Ri2Context())
            {
                if (string.IsNullOrEmpty(needle))
                {
                    res.AddRange(db.Profiles.Include(x => x.RcfProfile).Include(x => x.FideProfile)
                        .Include(x => x.Group));
                }
                else
                {
                    var tmp = db.Profiles
                        .Where(x => x.RcfProfile.Name.StartsWith(needle) || x.FideProfile.Name.StartsWith(needle) ||
                                    x.RcfProfile.RcfId.ToString() == needle ||
                                    x.FideProfile.FideId.ToString() == needle)
                        .Include(x => x.RcfProfile)
                        .Include(x => x.FideProfile)
                        .Include(x => x.Group);
                    res.AddRange(tmp);
                }
            }

            return res;
        }

        public void DeleteProfile(int id)
        {
            using (var db = new Ri2Context())
            {
                db.Profiles.Remove(
                    db.Profiles.SingleOrDefault(x => x.Id == id) ?? throw new InvalidOperationException());
                db.SaveChanges();
            }
        }

        public void SaveProfile(Profile profile)
        {
            using (var db = new Ri2Context())
            {
                var pr = db.Profiles.FirstOrDefault(x => x.Id == profile.Id);
                if (pr == null) throw new Exception("profile");
                var gr = db.Groups.FirstOrDefault(x => x.Id == profile.Group.Id);
                pr.Group = gr ?? throw new Exception("group");
                db.SaveChanges();
            }
        }

        public void MergeProfiles(Profile targetProfile, RcfProfile rcfProfile, FideProfile fideProfile)
        {
            using (var db = new Ri2Context())
            {
                targetProfile = db.Profiles.FirstOrDefault(x => x.Id == targetProfile.Id);
                if (targetProfile == null) return;
                if (rcfProfile != null)
                    rcfProfile = db.RcfProfiles.FirstOrDefault(x => x.Id == rcfProfile.Id);
                if (fideProfile != null)
                    fideProfile = db.FideProfiles.FirstOrDefault(x => x.Id == fideProfile.Id);
                if (rcfProfile != null) targetProfile.RcfProfile = rcfProfile;
                if (fideProfile != null)
                {
                    targetProfile.FideProfile = fideProfile;
                    if (rcfProfile != null) rcfProfile.FideProfile = fideProfile;
                }

                db.SaveChanges();
            }
        }

        public IEnumerable<RcfProfile> GetRcfProfiles()
        {
            using (var db = new Ri2Context())
            {
                return db.RcfProfiles.Include(x => x.FideProfile).ToList();
            }
        }

        public RcfProfile SearchRcfProfile(int rcfId)
        {
            using (var db = new Ri2Context())
            {
                return db.RcfProfiles.Include(x => x.FideProfile).FirstOrDefault(x => x.RcfId == rcfId);
            }
        }

        public IEnumerable<FideProfile> GetFideProfiles()
        {
            using (var db = new Ri2Context())
            {
                return db.FideProfiles.ToList();
            }
        }

        public FideProfile SearchFideProfile(int fideId)
        {
            using (var db = new Ri2Context())
            {
                return db.FideProfiles.FirstOrDefault(x => x.FideId == fideId);
            }
        }
    }
}