using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Ecommerce.Models
{
    public class LeftSideBarViewModel
    {
        public CategoryViewModel category { get; set; }
        public List<CategoryViewModel> categoryList { get; set; }
        public List<SubCategoryViewModel> subCategoryList { get; set; }
        public List<BrandViewModel> BrandList { get; set; }
        public List<SelectListItem> ColorList { get; set; }

    }
}