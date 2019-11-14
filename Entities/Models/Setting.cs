using System;
using System.Collections.Generic;

namespace Entities.Models
{
    public partial class Setting
    {
        public int Id { get; set; }
        public string Logo { get; set; }
        public string CompanyName { get; set; }
        public string PageTitle { get; set; }
        public string PageKeyword { get; set; }
        public string PageDescription { get; set; }
        public string FacebookLink { get; set; }
        public string TwitterLink { get; set; }
        public string GoogleLink { get; set; }
        public string Address { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string WebAddress { get; set; }
        public string Mon { get; set; }
        public string Tue { get; set; }
        public string Wed { get; set; }
        public string Thur { get; set; }
        public string Fri { get; set; }
        public string Sat { get; set; }
        public string Sun { get; set; }
        public bool isClosed { get; set; } 
    }
}
