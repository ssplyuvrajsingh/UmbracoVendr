using NPoco;
using Umbraco.Core.Persistence.DatabaseAnnotations;
using System;

namespace UmbracoVendr.App_Code.DatabaseRepo.DatabaseMigration
{
        [TableName("ProviderBankDetails")]
        [PrimaryKey("Id", AutoIncrement = true)]
        [ExplicitColumns]
        public class ProviderBankDetailsSchema
        {
            [PrimaryKeyColumn(AutoIncrement = true, IdentitySeed = 1)]
            [Column("Id")]
            public int Id { get; set; }

            [Column("ProviderId"), NullSetting]
            public int ProviderId { get; set; }

            [Column("BankAccountNumber"), NullSetting]
            public string BankAccountNumber { get; set; }

            [Column("IFSC"), NullSetting]
            public string IFSC { get; set; }

            [Column("AccountHolderName"), NullSetting]
            public string AccountHolderName { get; set; }

            [Column("LastUpdatedDate"), NullSetting]
            public DateTime LastUpdatedDate { get; set; }
        }
}