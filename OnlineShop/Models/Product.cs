using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OnlineShop.Models
{
    public class Product
    {
        [Key]
        public int ProductId { get; set; }

        [Display(Name = "Name")]
        public string ProductName { get; set; }

        [Display(Name = "Description")]
        public string ProductDescription { get; set; }

        [Display(Name = "Image")]
        public string MediaUrl { get; set; }

        [Display(Name = "Is approved")]
        public bool IsApproved { get; set; }

        public float Price { get; set; }

        [ForeignKey("Categories")]
        public int CategoryId { get; set; }

        [ForeignKey("ApplicationUser")]
        public string AuthorId { get; set; }

        public virtual ProductRequests ProductRequests { get; set; }
        public virtual Categories Categories { get; set; }
        public virtual ApplicationUser ApplicationUser { get; set; }
    }
}