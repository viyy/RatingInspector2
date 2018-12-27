using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Models
{
    public class Group : IdBaseEntity
    {
        [Required] public string Name { get; set; }

        public virtual List<Profile> Profiles { get; set; }
    }
}