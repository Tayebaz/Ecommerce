using System;
using System.Collections.Generic;

namespace Entities.Models
{
    public partial class Image
    {
        public int ImageId { get; set; }
        public int ProductId { get; set; }
        public string Images { get; set; }
        public virtual Product Product { get; set; }
    }
}
