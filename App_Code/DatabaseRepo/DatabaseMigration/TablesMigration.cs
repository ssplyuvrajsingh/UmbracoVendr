using Umbraco.Core.Logging;
using Umbraco.Core.Migrations;
using Vendr.Core;

namespace UmbracoVendr.App_Code.DatabaseRepo.DatabaseMigration
{
    public class AddCommentsTable : MigrationBase
    {
        StoredProcedure storedProcedure = new StoredProcedure();
        Triggers triggers = new Triggers();
        UmbracoSetups umbracoSetups = new UmbracoSetups();

        public AddCommentsTable(IMigrationContext context) : base(context)
        {
        }
        public override void Migrate()
        {   
            Logger.Debug<AddCommentsTable>("Running migration {MigrationStep}", "AddCommentsTable");

            // Lots of methods available in the MigrationBase class - discover with this.
            if (TableExists("ProductsTable") == false)
            {
                Create.Table<ProductsTableSchema>().Do();
            }
            else
            {
                Logger.Debug<AddCommentsTable>("The database table {DbTable} already exists, skipping", "ProductsTable");
            }

            if (TableExists("ProductFilesTable") == false)
            {
                Create.Table<ProductFilesTableSchema>().Do();
            }
            else
            {
                Logger.Debug<AddCommentsTable>("The database table {DbTable} already exists, skipping", "ProductsTable");
            }

            if (TableExists("ProviderBankDetails") == false)
            {
                Create.Table<ProviderBankDetailsSchema>().Do();
            }
            else
            {
                Logger.Debug<AddCommentsTable>("The database table {DbTable} already exists, skipping", "ProviderBankDetails");
            }

            if (TableExists("ProviderPaymentLogs") == false)
            {
                Create.Table<ProviderPaymentLogsSchema>().Do();
            }
            else
            {
                Logger.Debug<AddCommentsTable>("The database table {DbTable} already exists, skipping", "ProviderPaymentLogs");
            }

            if (TableExists("ProviderWallet") == false)
            {
                Create.Table<ProviderWalletSchema>().Do();
                ////Stored Procedure Created
                Database.Execute(Sql(storedProcedure.SpProductTableQuery()));
                Database.Execute(Sql(storedProcedure.SpProductWallet()));

                //Triggers Created
                Database.Execute(Sql(triggers.Tri_ProviderPaymentLogs_Insert_When_GetOrder()));
                Database.Execute(Sql(triggers.Tri_ProviderPaymentLogs_When_DeleteOrder()));
                Database.Execute(Sql(triggers.Tri_ProviderPaymentLogs_Update_When_OrderStatusCaptured()));
                Database.Execute(Sql(triggers.Tri_ProviderWallet_AddBalance()));
                Database.Execute(Sql(triggers.Tri_ProviderWallet_DebitBalance()));

                //Umbraco Setups
                Database.Execute(Sql(umbracoSetups.MemberPropertyAdd()));
                Database.Execute(Sql(umbracoSetups.UsnBlockComponents()));
                Database.Execute(Sql(umbracoSetups.OptionFormType()));
            }
            else
            {
                Logger.Debug<AddCommentsTable>("The database table {DbTable} already exists, skipping", "ProviderWallet");
            }
        }

    }
}