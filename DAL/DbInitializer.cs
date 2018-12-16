using System.Data.Entity;
using Models;
using Models.NotDb;

namespace DAL
{
    public class DbInitializer : CreateDatabaseIfNotExists<Ri2Context>
    {
        protected override void Seed(Ri2Context context)
        {
            context.Info.Add(new InfoValue {Name = InfoKeys.Version, Value = Ri2Context.Version});
            context.Groups.Add(new Group {Name = "Students"});
            base.Seed(context);
        }
    }
}