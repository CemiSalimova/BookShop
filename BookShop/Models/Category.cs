using System;
using System.Collections.Generic;

namespace BookShop.Models
{
    public partial class Category
    {
        public Category()
        {
            CategoryBooks = new HashSet<BookCategory>();
        }

        public int CategoryId { get; set; }
        public string Name { get; set; }

        public virtual ICollection<BookCategory> CategoryBooks { get; set; }
    }
}
