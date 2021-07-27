using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace USNWebsite.USNModels
{
    public class USNProductOrdersViewModel
    {
        public string FullName { get; set; }
        public string OrderStatusId { get; set; }
        public string OrderStatus { get; set; }
        public string OrderColor { get; set; }
        public string PaymentStatus { get; set; }
        public string Quantity { get; set; }
        public string Amount { get; set; }
        public string OrderId { get; set; }
        public DateTime? CreateDate { get; set; }
    }

    public class USNProductOrders
    {
        public List<USNProductOrdersViewModel> OrderList { get; set; }
        public List<OrderStatusModel> OrderStatusesDDL { get; set; }
    }

    public class OrderStatusModel
    {
        public string OrderStatusId { get; set; }
        public string OrderStatus { get; set; }
    }
}