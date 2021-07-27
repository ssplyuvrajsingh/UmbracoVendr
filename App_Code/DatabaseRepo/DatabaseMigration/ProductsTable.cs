using Umbraco.Core;
using Umbraco.Core.Logging;
using Umbraco.Core.Composing;
using Umbraco.Core.Migrations;
using Umbraco.Core.Migrations.Upgrade;
using Umbraco.Core.Scoping;
using Umbraco.Core.Services;
using NPoco;
using Umbraco.Core.Persistence.DatabaseAnnotations;
using System;

namespace UmbracoVendr.App_Code.DatabaseRepo.DatabaseMigration
{
    [RuntimeLevel(MinLevel = RuntimeLevel.Run)]
    public class ProductsTableComposer : ComponentComposer<ProductsTableComponent>
    {
    }

    public class ProductsTableComponent : IComponent
    {
        private IScopeProvider _scopeProvider;
        private IMigrationBuilder _migrationBuilder;
        private IKeyValueService _keyValueService;
        private ILogger _logger;

        public ProductsTableComponent(IScopeProvider scopeProvider, IMigrationBuilder migrationBuilder, IKeyValueService keyValueService, ILogger logger)
        {
            _scopeProvider = scopeProvider;
            _migrationBuilder = migrationBuilder;
            _keyValueService = keyValueService;
            _logger = logger;
        }

        public void Initialize()
        {
            // Create a migration plan for a specific project/feature
            // We can then track that latest migration state/step for this project/feature
            var migrationPlan = new MigrationPlan("ProductsTable");

            // This is the steps we need to take
            // Each step in the migration adds a unique value
            migrationPlan.From(string.Empty)
                .To<AddCommentsTable>("productstable-db");

            // Go and upgrade our site (Will check if it needs to do the work or not)
            // Based on the current/latest step
            var upgrader = new Upgrader(migrationPlan);
            upgrader.Execute(_scopeProvider, _migrationBuilder, _keyValueService, _logger);
        }

        public void Terminate()
        {
        }
    }


    [TableName("ProductsTable")]
    [PrimaryKey("ProductId", AutoIncrement = true)]
    [ExplicitColumns]
    public class ProductsTableSchema
    {
        [PrimaryKeyColumn(AutoIncrement = true, IdentitySeed = 1)]
        [Column("ProductId")]
        public int ProductId { get; set; }

        [Column("MemberId")]
        public int MemberId { get; set; }

        [Column("ProductName"), NullSetting]
        public string ProductName { get; set; }

        [Column("ProductType"), NullSetting]
        public string ProductType { get; set; }

        [Column("ProductPrice"), NullSetting]
        public float ProductPrice { get; set; }

        [Column("ProductDescription"), NullSetting]
        public string ProductDescription { get; set; }

        [Column("ProductCreation")]
        public DateTime ProductCreation { get; set; }

        [Column("ProductLogoLarge"), NullSetting]
        public string ProductLogoLarge { get; set; }

        [Column("ProductLogoSmall"), NullSetting]
        public string ProductLogoSmall { get; set; }

        [Column("ProductScreenshot"), NullSetting]
        public string ProductScreenshot { get; set; }

        [Column("ProductScreenshotDescription"), NullSetting]
        public string ProductScreenshotDescription { get; set; }

        [Column("ProductProtectionTrialLength"), NullSetting]
        public int ProductProtectionTrialLength { get; set; }

        [Column("ProductProtectionNumberOfTrials"), NullSetting]
        public int ProductProtectionNumberOfTrials { get; set; }

        [Column("ProductStatus"), NullSetting]
        public string ProductStatus { get; set; }

        [Column("ProductNodeId"), NullSetting]
        public int ProductNodeId { get; set; }

        [Column("ProductNodeKey"),NullSetting]
        public string ProductNodeKey { get; set; }
    }
}
