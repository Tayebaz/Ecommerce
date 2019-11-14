using System;
using System.Collections.Generic;

namespace Entities.Models
{
    public partial class Review
    {
        public int ReviewId { get; set; }
        public int UserId { get; set; }
        public string Review1 { get; set; }
        public System.DateTime CreatedDate { get; set; }
        public int Rating { get; set; }
        public int ProductId { get; set; }
    }
}
