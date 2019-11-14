using System;
using System.Collections.Generic;

namespace Entities.Models
{
    public partial class WishList
    {
        public int WishListId { get; set; }
        public int ProductId { get; set; }
        public int UserId { get; set; }
        public Nullable<int> Size { get; set; }
        public string Attributes { get; set; }
    }
}
