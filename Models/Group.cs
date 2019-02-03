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
            return Id == other.Id;
        }

        public override bool Equals(object obj)
        {
            return obj is Group a && Equals(a);
        }

        public override int GetHashCode()
        {
            return Id;
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