using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Ecommerce.Models
{
    public class HelpViewModel
    {
        public int Id { get; set; }
        public string TokenKey { get; set; }
        [Required]
        public string Contents { get; set; }
        public string ContentType { get; set; }
    }
}