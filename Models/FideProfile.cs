using System.ComponentModel.DataAnnotations;

namespace Models
{
    public class FideProfile : IdBaseEntity
    {
        [Required]
        public int FideId { get; set; }
        [Required]
        public string Name { get; set; }
        public int Birth { get; set; }
        public int Std { get; set; } = 0;
        public int Rpd { get; set; } = 0;
        public int Blz { get; set; } = 0;
    }
}