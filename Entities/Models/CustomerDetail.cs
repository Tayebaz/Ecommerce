using System;
using System.Collections.Generic;

namespace Entities.Models
{
    public partial class CustomerDetail
    {
        public int CustomerId { get; set; }
        public string EmaiId { get; set; }
        public string Password { get; set; }
        public string Address { get; set; }
        public string Contact { get; set; }
        public string CustomerName { get; set; }
        public string Encid { get; set; }
        public bool IsBlocked { get; set; }
    }
}
