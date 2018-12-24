using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OnlineShop.Models
{
    public class UserRights
    {
        [Required]
        [Key, ForeignKey("ApplicationUser")]
        public string UserId { get; set; }
        [Display(Name = "Can buy?")]
        public bool CanBuy { get; set; }
        [Display(Name = "Can review?")]
        public bool CanReview { get; set; }
        public virtual ApplicationUser ApplicationUser { get; set; }
    }
}