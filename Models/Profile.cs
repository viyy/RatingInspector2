using System.ComponentModel.DataAnnotations.Schema;

namespace Models
{
    public class Profile : IdBaseEntity
    {
        public int? RcfProfileId { get; set; }

        [ForeignKey("RcfProfileId")] public virtual RcfProfile RcfProfile { get; set; }

        public int? FideProfileId { get; set; }

        [ForeignKey("FideProfileId")] public virtual FideProfile FideProfile { get; set; }

        public int GroupId { get; set; }

        [ForeignKey("GroupId")] public virtual Group Group { get; set; }

        [NotMapped] public int Birth => RcfProfile?.Birth ?? FideProfile?.Birth ?? 0;
    }
}