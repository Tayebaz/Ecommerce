using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Ecommerce.Models
{
    public class CategoryViewModel
    {
        public int CategoryId { get; set; }
        public string TokenKey { get; set; }
        [Required]
        [Display(Name="Category Name")]
        public string CategoryName { get; set; }
        public List<SubCategoryViewModel> subCategoryList { get; set; }
        public List<ProductViewModel> productList { get; set; }
    }
}