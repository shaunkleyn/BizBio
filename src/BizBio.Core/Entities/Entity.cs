using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BizBio.Core.Entities
{
    /// <summary>
    /// Unifies users and organizations/profiles for business card relationships
    /// Used to create relationships like "works_at", "manages", "colleague", "client_of", etc.
    /// This allows both individuals and organizations to have relationships with each other
    /// </summary>
    public class Entity : BaseEntity
    {
        // EntityType: 1 = User, 2 = Profile/Organization
        public int EntityTypeId { get; set; }

        // ReferenceId points to either User.Id or Profile.Id depending on EntityTypeId
        public int ReferenceId { get; set; }

        [MaxLength(255)]
        public string? DisplayName { get; set; }

        // Navigation properties
        public virtual ICollection<EntityRelationship> RelationshipsFrom { get; set; } = new List<EntityRelationship>();
        public virtual ICollection<EntityRelationship> RelationshipsTo { get; set; } = new List<EntityRelationship>();
    }
}
