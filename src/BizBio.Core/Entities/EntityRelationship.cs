using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BizBio.Core.Entities
{
    /// <summary>
    /// Represents relationships between entities (users and organizations) for business cards
    /// Examples:
    /// - User 1 -> Company A: owner_of
    /// - Chris -> Company A: works_at
    /// - Chris -> User 1: reports_to
    /// - User 1 -> Chris: manages
    /// - Chris -> Stacy: colleague, friend_of, client_of
    /// - Company A -> John: employs
    /// </summary>
    public class EntityRelationship : BaseEntity
    {
        //| From      | To        | Type       |
        //| --------- | --------- | ---------- |
        //| User 1    | Company A | owner_of   |
        //| Chris     | Company A | works_at   |
        //| Chris     | User 1    | reports_to |
        //| User 1    | Chris     | manages    |
        //| Chris     | Stacy     | colleague  |
        //| Company A | John      | employs    |

        public int FromEntityId { get; set; }
        public int ToEntityId { get; set; }

        // RelationshipTypeId: Reference to lookup table
        // Types: works_at, reports_to, manages, colleague, friend_of, client_of, vendor_of, partner_of, etc.
        public int RelationshipTypeId { get; set; }

        [MaxLength(500)]
        public string? Description { get; set; }

        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }

        public bool IsVisible { get; set; } = true; // Can hide relationships from public view

        // Navigation properties
        public virtual Entity FromEntity { get; set; } = null!;
        public virtual Entity ToEntity { get; set; } = null!;
    }
}
