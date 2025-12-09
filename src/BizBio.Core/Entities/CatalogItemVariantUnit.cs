using BizBio.Core.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BizBio.Core.Entities
{
    public class CatalogItemVariantUnit : BaseEntity
    {
        //public int Id { get; set; }

        public int UnitTypeId { get; set; }     // enum: Size, Weight, Volume, Count, Length

        public double? Value { get; set; }       // 300ml → 300, 250g → 250, pack of 100 → 100

        public string Label { get; set; }        // "Small", "500ml", "250g", "Pack of 100"
    }
}
