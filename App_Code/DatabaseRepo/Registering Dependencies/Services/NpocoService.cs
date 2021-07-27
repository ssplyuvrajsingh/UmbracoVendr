using NPoco;
using System.Configuration;
using System.Data.SqlClient;

namespace Providers.Services
{
    public class NpocoService : INpocoService
    {
        private string connectionString;
        public NpocoService()
        {
            connectionString = ConfigurationManager.ConnectionStrings["umbracoDbDSN"].ConnectionString;
        }

        public IDatabase Database()
        {
            return new Database(connectionString, DatabaseType.SqlServer2012, SqlClientFactory.Instance);
        }
    }
}