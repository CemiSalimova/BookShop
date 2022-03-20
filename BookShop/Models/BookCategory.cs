﻿using System;
using System.Collections.Generic;

namespace BookShop.Models
{
    public partial class BookCategory
    {
        public int BookId { get; set; }
        public int CategoryId { get; set; }

        public virtual Book Book { get; set; }
        public virtual Category Category { get; set; }
    }
}
