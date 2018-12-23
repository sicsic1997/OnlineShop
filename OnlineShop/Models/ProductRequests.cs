using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace OnlineShop.Models
{
    public class ProductRequests
    {
        [Key, ForeignKey("Product")]
        public int ProductId { get; set; }

        [Display(Name = "Product request description")]
        public string ProductRequestsDescription { get; set; }

        [Display(Name = "Is approved")]
        public bool IsApproved { get; set; }


        public virtual Product Product { get; set; }

    }
}