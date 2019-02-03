using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using DAL;
using Interfaces;
using Models;

namespace Services
{
    public class GroupManagerService : IGroupManager
    {
        public List<Group> GetGroups(bool fullData = false)
        {
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
            using (var db = new Ri2Context())
            {
                var tmp = db.Groups.Include(x => x.Profiles).FirstOrDefault(x => x.Id == gr.Id);
                if (tmp == null || tmp.Profiles.Count != 0) return;
                db.Groups.Remove(tmp);
                db.SaveChanges();
            }
        }

        public void CreateGroup(Group gr)
        {
            using (var db = new Ri2Context())
            {
                gr.Id = 0;
                db.Groups.Attach(gr);
                db.Entry(gr).State = EntityState.Added;
                db.SaveChanges();
            }
        }

        public void DeleteGroupWithProfiles(int id)
        {
            using (var db = new Ri2Context())
            {
                var gr = db.Groups.Include(x => x.Profiles).FirstOrDefault(x => x.Id == id);
                if (gr == null) return;
                db.Profiles.RemoveRange(gr.Profiles);
                db.Groups.Remove(gr);
                db.SaveChanges();
            }
        }
    }
}