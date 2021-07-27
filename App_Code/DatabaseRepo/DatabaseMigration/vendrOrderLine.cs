using System;
using System.Web.DynamicData;


namespace UmbracoVendr.App_Code.DatabaseRepo.DatabaseMigration
{
    [TableName("vendrOrderLine")]
    public class VendrOrderLine
    {
        public Guid OrderId { get; set; }
        public string ProductReference { get; set; }
    }
}