using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Ecommerce.Models
{
    public class AddtoCartViewModel
    {
        public int ID { get; set; }
        public string encid { get; set; }
        public long ProductId { get; set; }
        public string  Product { get; set; }
    }
}