using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FinCMS.App_Code.Models.Dtos
{
    public class AddToCartDto
    {
        public string ProductReference { get; set; }
        public decimal Quantity { get; set; }
    }
}