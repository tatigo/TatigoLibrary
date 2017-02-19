using System.Configuration;

namespace TatigoLibrary.Data
{
    public class BaseConfig
    {
        public const string DATA_CONNECTION_CONFIG = "DataConnectionString";

        public const string DATA_TEST_CONNECTION_CONFIG = "DataTestConnectionString";

        public static string GetDataConnectionString
        {
            get
            {
                return ConfigurationManager.ConnectionStrings[DATA_CONNECTION_CONFIG].ToString();
            }
        }

        public static string GetDataTestConnectionString
        {
            get
            {
                return ConfigurationManager.ConnectionStrings[DATA_TEST_CONNECTION_CONFIG].ConnectionString;
            }
        }
    }
}