using System;
using System.Collections.Generic;

namespace Entities.Models
{
    public partial class Category
    {
        public Category()
        {
            this.Brands = new List<Brand>();
            this.SubCategories = new List<SubCategory>();
        }

        public int CategoryId { get; set; }
        public string TokenKey { get; set; }
        public string CategoryName { get; set; }
        public virtual ICollection<Brand> Brands { get; set; }
        public virtual ICollection<SubCategory> SubCategories { get; set; }
    }
}
