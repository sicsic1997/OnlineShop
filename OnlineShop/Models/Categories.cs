using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace OnlineShop.Models
{
    public class Categories
    {
        [Key]
        public int CategoriesId { get; set; }

        [Display(Name = "Category")]
        public string Description { get; set; }

        public virtual ICollection<Product> Products { get; set; }

    }
}