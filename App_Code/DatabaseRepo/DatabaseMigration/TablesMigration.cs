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

            #region tables

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
            }
            else
            {
                Logger.Debug<AddCommentsTable>("The database table {DbTable} already exists, skipping", "ProviderWallet");
            }

            #endregion

            #region stored procedure created 

            var count = Database.FirstOrDefault<int>(Sql(storedProcedure.ExistSpProductTableQuery()));
            if (count == 0)
            {
                Database.Execute(Sql(storedProcedure.SpProductTableQuery()));
            }

            count = Database.FirstOrDefault<int>(Sql(storedProcedure.ExistSpProductWallet()));
            if (count == 0)
            {
                Database.Execute(Sql(storedProcedure.SpProductWallet()));
            }

            #endregion

            #region triggers created

            count = Database.FirstOrDefault<int>(Sql(triggers.ExistsTri_ProviderPaymentLogs_Insert_When_GetOrder()));
            if (count == 0)
            {
                Database.Execute(Sql(triggers.Tri_ProviderPaymentLogs_Insert_When_GetOrder()));
            }

            count = Database.FirstOrDefault<int>(Sql(triggers.ExistsTri_ProviderPaymentLogs_When_DeleteOrder()));
            if (count == 0)
            {
                Database.Execute(Sql(triggers.Tri_ProviderPaymentLogs_When_DeleteOrder()));
            }

            count = Database.FirstOrDefault<int>(Sql(triggers.ExistsTri_ProviderPaymentLogs_Update_When_OrderStatusCaptured()));
            if (count == 0)
            {
                Database.Execute(Sql(triggers.Tri_ProviderPaymentLogs_Update_When_OrderStatusCaptured()));
            }

            count = Database.FirstOrDefault<int>(Sql(triggers.ExistsTri_ProviderWallet_AddBalance()));
            if (count == 0)
            {
                Database.Execute(Sql(triggers.Tri_ProviderWallet_AddBalance()));
            }

            count = Database.FirstOrDefault<int>(Sql(triggers.ExistsTri_ProviderWallet_DebitBalance()));
            if (count == 0)
            {
                Database.Execute(Sql(triggers.Tri_ProviderWallet_DebitBalance()));
            }

            #endregion

            #region umbraco setups

            Database.Execute(Sql(umbracoSetups.MemberPropertyAdd()));

            var config = Database.FirstOrDefault<string>(Sql(umbracoSetups.GetUsnBlockComponents()));
            if (config != null)
            {
                Database.Execute(Sql(umbracoSetups.UsnBlockComponents(config)));
            }

            config = Database.FirstOrDefault<string>(Sql(umbracoSetups.GetOptionFormType()));
            if (config != null)
            {
                Database.Execute(Sql(umbracoSetups.OptionFormType(config)));
            }

            #endregion
        }

    }
}