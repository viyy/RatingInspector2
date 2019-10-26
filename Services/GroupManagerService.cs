using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using Common;
using DAL;
using Interfaces;
using Models;
using Nelfias.License;
using License = Common.License;

namespace Services
{
    public class GroupManagerService : IGroupManager
    {
        public List<Group> GetGroups(bool fullData = false)
        {
            Logger.Log("GroupManagerService", "Retrieving Groups");
            using (var db = new Ri2Context())
            {
                return fullData
                    ? db.Groups.Include(x => x.Profiles).Include(x => x.Profiles.Select(y => y.FideProfile))
                        .Include(x => x.Profiles.Select(y => y.RcfProfile)).ToList()
                    : db.Groups.ToList();
            }
        }

        public void UpdateGroup(Group gr)
        {
            Logger.Log("GroupManagerService", $"Updating group [{gr.Id}]{gr.Name}");
            using (var db = new Ri2Context())
            {
                var tmp = db.Groups.FirstOrDefault(x => x.Id == gr.Id);
                if (tmp == null) return;
                tmp.Name = gr.Name;
                db.SaveChanges();
            }
        }

        public async Task MergeGroups(Group from, Group to)
        {
            Logger.Log("GroupManagerService", $"Merging groups [{from.Id}]{from.Name} and [{to.Id}]{to.Name}");
            using (var db = new Ri2Context())
            {
                var origin = db.Groups.FirstOrDefault(x => x.Id == from.Id);
                if (origin == null) return;
                var target = db.Groups.FirstOrDefault(x => x.Id == to.Id);
                if (target == null) return;
                await Task.Run(() =>
                {
                    foreach (var profile in origin.Profiles) profile.Group = target;
                }).ConfigureAwait(false);
                db.SaveChanges();
            }
        }

        public void DeleteGroup(Group gr)
        {
            Logger.Log("GroupManagerService", $"Deleting group [{gr.Id}]{gr.Name}");
            using (var db = new Ri2Context())
            {
                var tmp = db.Groups.Include(x => x.Profiles).FirstOrDefault(x => x.Id == gr.Id);
                var forced = false;
                if (ConfigurationManager.AppSettings["forcedgroupdelete"] != null)
                {
                    bool.TryParse(ConfigurationManager.AppSettings["forcedgroupdelete"], out forced);
                }

                if (tmp == null)
                {
                    Logger.Log("GroupManagerService", "Group not found", LogLevel.Error);
                    return;
                }

                if (!forced)
                {
                    if (tmp.Profiles.Count != 0)
                    {
                        Logger.Log("GroupManagerService", $"Group [{gr.Id}]{gr.Name} not empty. Can not delete.",
                            LogLevel.Warning);
                        return;
                    }
                }
                else
                {
                    Logger.Log("GroupManagerService", "Forced mode enabled", LogLevel.Warning);
                    DeleteGroupWithProfiles(gr.Id);
                    return;
                }

                db.Groups.Remove(tmp);
                db.SaveChanges();
            }
        }

        public void CreateGroup(Group gr)
        {
            try
            {
                // License block++
                var limit = License.GetData("groups");
                if (!int.TryParse(limit, out var limitResult))
                {
                    throw new InvalidLicenseException("Group Manager: Invalid License Data");
                }

                // License block--
                using (var db = new Ri2Context())
                {
                    if (limitResult != -1 && db.Groups.Count() >= limitResult)
                    {
                        throw new OutOfLicenseLimitException("Group Limit Reached");
                    }

                    gr.Id = 0;
                    db.Groups.Attach(gr);
                    db.Entry(gr).State = EntityState.Added;
                    db.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                Logger.Log("GroupManagerService", $"Error: {ex.Message}", LogLevel.Error);
            }
        }

        public void DeleteGroupWithProfiles(int id)
        {
            Logger.Log("GroupManagerService", $"Deleting group [{id}] --forced", LogLevel.Warning);
            using (var db = new Ri2Context())
            {
                var gr = db.Groups.Include(x => x.Profiles).FirstOrDefault(x => x.Id == id);
                if (gr == null)
                {
                    Logger.Log("GroupManagerService", "Group not found", LogLevel.Error);
                    return;
                }
                db.Profiles.RemoveRange(gr.Profiles);
                db.Groups.Remove(gr);
                db.SaveChanges();
            }
        }
    }
}