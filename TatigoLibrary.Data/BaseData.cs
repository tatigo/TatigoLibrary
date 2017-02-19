using PetaPoco;
using System;
using System.Data.SqlClient;

namespace TatigoLibrary.Data
{
    public class BaseDataAccess
    {
        public BaseDataAccess(string InConnectionString, bool InUsePetaPoco = true)
        {
            this.ConnectionString = ConnectionString;

            if (InUsePetaPoco)
                this.PetaPocoContext = new Database(this.ConnectionString, "System.Data.SqlClient");
        }

        public BaseDataAccess(DataBaseTypes InDBType, bool InUsePetaPoco = true)
        {
            switch (InDBType)
            {
                case DataBaseTypes.DATA:
                    {
                        this.ConnectionString = BaseConfig.GetDataConnectionString;
                        break;
                    }
                case DataBaseTypes.DATA_TEST:
                    {
                        this.ConnectionString = BaseConfig.GetDataTestConnectionString;
                        break;
                    }
                default:
                    {
                        throw new NotImplementedException(string.Format("DataBase of type [{0}] was not implemented.", InDBType.ToString()));
                    }
            }
            if (InUsePetaPoco)
            {
                this.PetaPocoContext = new Database(this.ConnectionString, "System.Data.SqlClient");
            }
        }

        public string ConnectionString { get; private set; }

        public Database PetaPocoContext { get; private set; }

        public static string AppendConnectionTimeOut(int InConnectionTimeOut, SqlConnection InConnection)
        {
            var sqlConnectionStringBuilder = new SqlConnectionStringBuilder()
            {
                ConnectionString = InConnection.ConnectionString,
                ConnectTimeout = InConnectionTimeOut
            };

            return sqlConnectionStringBuilder.ToString();
        }

        public static int GetConnectionTimeOut(SqlConnection InConnection)
        {
            return (new SqlConnectionStringBuilder() { ConnectionString = InConnection.ConnectionString }).ConnectTimeout;
        }
    }

    public class BaseDataContext
    {
        [ThreadStatic]
        private readonly static BaseDataContext s_instance;

        static BaseDataContext()
        {
            BaseDataContext.s_instance = new BaseDataContext();
        }

        protected BaseDataContext(DataBaseTypes InDBtype = DataBaseTypes.DATA)
        {
            this.DataBase = new BaseDataAccess(InDBtype, true);
        }

        public Database DataContext
        {
            get { return this.DataBase.PetaPocoContext; }
        }

        private BaseDataAccess DataBase
        {
            get;
            set;
        }

        public static BaseDataAccess GetDataContext()
        {
            BaseDataAccess dataContext;

            try
            {
                dataContext = BaseDataContext.s_instance.DataBase;
            }
            finally
            {
                if (BaseDataContext.s_instance ==null)
                    dataContext = (new BaseDataContext()).DataBase;
            }

            return dataContext;
        }
    }
}