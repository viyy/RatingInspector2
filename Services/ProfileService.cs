using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using Common;
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
            try
            {
                Logger.Log("ProfileService", $"Retrieving profiles");
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
            }
            catch (Exception ex)
            {
                Logger.Log("ProfileService", $"Error: {ex.Message}", LogLevel.Error);
            }
            return res;
        }

        public void DeleteProfile(int id)
        {
            try
            {
                Logger.Log("ProfileService", $"Deleting profile [{id}]");
                using (var db = new Ri2Context())
                {
                    db.Profiles.Remove(
                        db.Profiles.SingleOrDefault(x => x.Id == id) ?? throw new InvalidOperationException());
                    db.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                Logger.Log("ProfileService", $"Error: {ex.Message}", LogLevel.Error);
            }
        }

        public void SaveProfile(Profile profile)
        {
            try
            {
                using (var db = new Ri2Context())
                {
                    var pr = db.Profiles.FirstOrDefault(x => x.Id == profile.Id);
                    if (pr == null)
                    {
                        Logger.Log("ProfileService", $"Profile [{profile.Id}] not found", LogLevel.Error);
                        throw new Exception("profile");
                    }

                    var gr = db.Groups.FirstOrDefault(x => x.Id == profile.Group.Id);
                    if (gr == null)
                    {
                        Logger.Log("ProfileService", $"Group [{profile.Group.Id}] not found", LogLevel.Error);
                    }

                    pr.Group = gr ?? throw new Exception("group");
                    db.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                Logger.Log("ProfileService", $"Error: {ex.Message}", LogLevel.Error);
            }
        }

        public void MergeProfiles(Profile targetProfile, RcfProfile rcfProfile, FideProfile fideProfile)
        {
            try
            {
                Logger.Log("ProfileService",
                    $"Merging [{targetProfile.Id}]: Rcf[{rcfProfile?.RcfId ?? -1}], Fide[{fideProfile?.FideId ?? -1}]");
                //License block++
                if (License.GetData("merge") != "true")
                {
                    throw new OutOfLicenseLimitException(
                        "Profiles: You can not merge profiles with your current license");
                }

                //License block--
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
            catch (Exception ex)
            {
                Logger.Log("ProfileService", $"Error: {ex.Message}", LogLevel.Error);
            }
        }

        public IEnumerable<RcfProfile> GetRcfProfiles()
        {
            Logger.Log("ProfileService", $"Retrieving rcf profiles");
            using (var db = new Ri2Context())
            {
                return db.RcfProfiles.Include(x => x.FideProfile).ToList();
            }
        }

        public RcfProfile SearchRcfProfile(int rcfId)
        {
            Logger.Log("ProfileService", $"Retrieving rcf profile [{rcfId}]");
            using (var db = new Ri2Context())
            {
                return db.RcfProfiles.Include(x => x.FideProfile).FirstOrDefault(x => x.RcfId == rcfId);
            }
        }

        public IEnumerable<FideProfile> GetFideProfiles()
        {
            Logger.Log("ProfileService", $"Retrieving fide profiles");
            using (var db = new Ri2Context())
            {
                return db.FideProfiles.ToList();
            }
        }

        public FideProfile SearchFideProfile(int fideId)
        {
            Logger.Log("ProfileService", $"Retrieving fide profile [{fideId}]");
            using (var db = new Ri2Context())
            {
                return db.FideProfiles.FirstOrDefault(x => x.FideId == fideId);
            }
        }
    }
}