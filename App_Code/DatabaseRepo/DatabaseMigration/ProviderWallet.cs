using NPoco;
using Umbraco.Core.Persistence.DatabaseAnnotations;
using System;

namespace UmbracoVendr.App_Code.DatabaseRepo.DatabaseMigration
{
    [TableName("ProviderWallet")]
    [PrimaryKey("Id", AutoIncrement = true)]
    [ExplicitColumns]
    public class ProviderWalletSchema
    {
        [PrimaryKeyColumn(AutoIncrement = true, IdentitySeed = 1)]
        [Column("Id")]
        public int Id { get; set; }

        [Column("ProviderId"), NullSetting]
        public int ProviderId { get; set; }

        [Column("ProviderBalance"), NullSetting]
        public decimal ProviderBalance { get; set; }

        [Column("WithdrawalPending"), NullSetting]
        public decimal WithdrawalPending { get; set; }

        [Column("TotalPaid"), NullSetting]
        public decimal TotalPaid { get; set; }

        [Column("LastUpdatedDate"), NullSetting]
        public DateTime LastUpdatedDate { get; set; }
    }
}