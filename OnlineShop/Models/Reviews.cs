using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace OnlineShop.Models
{
    public class Reviews
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [Range(0, 5)]
        public int Rating { get; set; }

        [Required]
        [MinLength(10)]
        [MaxLength(500)]
        public string Comment { get; set; }

        [ForeignKey("Product")]
        public int ProductId { get; set; }

        [ForeignKey("ApplicationUser")]
        public string AuthorId { get; set; }

        public virtual Product Product { get; set; }
        public virtual ApplicationUser ApplicationUser { get; set; }
    }
}