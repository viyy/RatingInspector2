using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Models
{
    public class RcfProfile : IdBaseEntity
    {
        [Required]
        public int RcfId { get; set; }
        [Required]
        public string Name { get; set; }
        public int Birth { get; set; }
        public int Std { get; set; } = 0;
        public int Rpd { get; set; } = 0;
        public int Blz { get; set; } = 0;

        public int? FideProfileId { get; set; }
        [ForeignKey("FideProfileId")]
        public virtual FideProfile FideProfile { get; set; }
    }
}