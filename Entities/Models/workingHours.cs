using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Models
{
    public class workingHours
    {
        public string ID { get; set; }
        public string OpenTime { get; set; }
        public string CloseTime { get; set; }
        public bool isClosedAllDay { get; set; }
    }
}
