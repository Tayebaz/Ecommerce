using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Ecommerce.Models
{
    public class SubCategoryViewModel
    {
        public int SubCategoryId { get; set; }
        public string TokenKey { get; set; }
        public string SubCategoryName { get; set; }     
        public int CategoryId { get; set; }
        public string CategoryName { get; set; }
        public List<System.Web.Mvc.SelectListItem> CategoryList { get; set; }
        public List<ProductViewModel> productList { get; set; }
    }
}