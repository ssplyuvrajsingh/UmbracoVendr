using System;
using System.Web.DynamicData;

namespace UmbracoVendr.App_Code.DatabaseRepo.DatabaseMigration
{
    [TableName("vendrOrder")]
    public class VendrOrder
    {
        public string Id { get; set; }
        public string OrderNumber  { get; set; }
        public string CustomerReference { get; set; }
        public string CustomerFirstName { get; set; }
        public string CustomerLastName { get; set; }
        public string CustomerEmail { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime UpdateDate { get; set; }
        public DateTime FinalizedDate { get; set; }
        public string PaymentMethodId { get; set; }
        public string TransactionId { get; set; }
        public int PaymentStatus { get; set; }
    }
}