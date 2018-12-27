using System.ComponentModel.DataAnnotations;

namespace Models
{
    public class FideProfile : IdBaseEntity
    {
        [Required] public int FideId { get; set; } = 0;

        [Required] public string Name { get; set; } = "";

        public int Birth { get; set; } = 0;
        public int Std { get; set; } = 0;
        public int Rpd { get; set; } = 0;
        public int Blz { get; set; } = 0;
    }
}