using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Ecommerce.Models
{
    public class BrandViewModel
    {
        public int BrandId { get; set; }
        public string TokenKey { get; set; }
        [Required]
        [Display(Name = "Brand Name")]
        public string BrandName { get; set; }      
         [Required]
        [Display(Name = "Category")]
        public int CategoryId { get; set; }
         [Display(Name = "Category")]
        public string CategoryName { get; set; }
        public List<System.Web.Mvc.SelectListItem> CategoryList { get; set; }
        public int ProductCount { get; set; }
    }
}