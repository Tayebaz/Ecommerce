using System;
using System.Collections.Generic;

namespace Entities.Models
{
    public partial class SubCategory
    {
        public int SubCategoryId { get; set; }
        public string TokenKey { get; set; }
        public string SubCategoryName { get; set; }
        public int CategoryId { get; set; }
        public virtual Category Category { get; set; }
    }
}
