using System;
using System.Collections.Generic;

namespace Entities.Models
{
    public partial class SliderSetting
    {
        public int SliderId { get; set; }
        public string SliderImage { get; set; }
        public Nullable<int> ImageOrder { get; set; }
        public string TokenKey { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
    }
}
