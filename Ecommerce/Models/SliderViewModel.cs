using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Ecommerce.Models
{
    public class SliderViewModel
    {
        public string TokenKey { get; set; }
        public int SliderId { get; set; }
        public string SliderImage { get; set; }
        public HttpPostedFileBase SliderImageUpload { get; set; }
        public Nullable<int> ImageOrder { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
    }
}