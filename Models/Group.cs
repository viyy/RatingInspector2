using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Models
{
    public class Group : IdBaseEntity, IEquatable<Group>
    {
        [Required] public string Name { get; set; }

        public virtual List<Profile> Profiles { get; set; }

       public bool Equals(Group other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return string.Equals(Name, other.Name) && Id == other.Id;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((Name != null ? Name.GetHashCode() : 0) * 397) ^ Id;
            }
        }

        public static bool operator ==(Group left, Group right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(Group left, Group right)
        {
            return !Equals(left, right);
        }
    }
}