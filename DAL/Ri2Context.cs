using System;
using System.Data.Entity;
using Common;
using Models;

namespace DAL
{
    public class Ri2Context : DbContext
    {
        public const string Version = "1.0";

        public Ri2Context() : base("DefaultConnection")
        {
            try
            {
                Database.SetInitializer(new DbInitializer());
            }
            catch (Exception ex)
            {
                Logger.Log("DAL", $"Initialization Error: {ex.Message}", LogLevel.Error);
            }

        }

        public DbSet<InfoValue> Info { get; set; }
        public DbSet<Group> Groups { get; set; }
        public DbSet<FideProfile> FideProfiles { get; set; }
        public DbSet<RcfProfile> RcfProfiles { get; set; }
        public DbSet<Profile> Profiles { get; set; }
    }
}