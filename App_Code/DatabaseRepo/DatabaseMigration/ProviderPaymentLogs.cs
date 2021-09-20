using System;
using NPoco;
using Umbraco.Core.Persistence.DatabaseAnnotations;

namespace UmbracoVendr.App_Code.DatabaseRepo.DatabaseMigration
{
    [TableName("ProviderPaymentLogs")]
    [PrimaryKey("LogId", AutoIncrement = true)]
    [ExplicitColumns]
    public class ProviderPaymentLogsSchema
    {
        [PrimaryKeyColumn(AutoIncrement = true, IdentitySeed = 1)]
        [Column("LogId")]
        public int LogId { get; set; }

        [Column("ProviderId"), NullSetting]
        public int ProviderId { get; set; }

        [Column("Description"), NullSetting]
        public string Description { get; set; }

        [Column("Amount"), NullSetting]
        public decimal Amount { get; set; }

        [Column("LogDate"), NullSetting]
        public DateTime LogDate { get; set; }

        [Column("PaymentStatus"), NullSetting]
        public string PaymentStatus { get; set; }

        [Column("OrderId"), NullSetting]
        public string OrderId { get; set; }

        [Column("AmountFlag"), NullSetting]
        public string AmountFlag { get; set; }
    }
}