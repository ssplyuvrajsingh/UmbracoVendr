using NPoco;
using Umbraco.Core.Persistence.DatabaseAnnotations;
using System;

namespace UmbracoVendr.App_Code.DatabaseRepo.DatabaseMigration
{
    [TableName("ProductFilesTable")]
    [PrimaryKey("ProductFileId", AutoIncrement = true)]
    [ExplicitColumns]
    public class ProductFilesTableSchema
    {
        [PrimaryKeyColumn(AutoIncrement = true, IdentitySeed = 1)]
        [Column("ProductFileId")]
        public int ProductFileId { get; set; }

        [Column("ProductId")]
        public int ProductId { get; set; }

        [Column("ProductFile")]
        public string ProductFile { get; set; }

        [Column("ProductVersionNumber"), NullSetting]
        public string ProductVersionNumber { get; set; }

        [Column("CreatedVersionDateTime"), NullSetting]
        public DateTime CreatedVersionDateTime { get; set; }
    }
}