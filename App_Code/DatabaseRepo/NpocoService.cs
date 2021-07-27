//using NPoco;
//using System.Configuration;
//using System.Data.SqlClient;

//public class NpocoService
//{
//    private string connectionString;
//    public NpocoService()
//    {
//        connectionString = ConfigurationManager.ConnectionStrings["umbracoDbDSN"].ConnectionString;
//    }

//    public IDatabase database
//    {
//        get
//        {
//            return new Database(connectionString, DatabaseType.SqlServer2012, SqlClientFactory.Instance);
//        }
//    }
//}