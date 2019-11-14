using System;
using System.Collections.Generic;

namespace Entities.Models
{
    public partial class Help
    {
        public int Id { get; set; }
        public string TokenKey { get; set; }
        public string Contents { get; set; }
        public string ContentType { get; set; }
    }
}
