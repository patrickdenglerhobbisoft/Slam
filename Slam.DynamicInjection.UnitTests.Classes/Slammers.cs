//
//using System;
//using System.Collections.Generic;
//using System.Data.SqlClient;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;


//namespace Hobbisoft.Slam.DynamicInjection
//{

//    #region SUPPORTING CLASSES AND STRUCTS


//    public enum ExecuteType { NonQuery }
//    public enum DatabaseAccessAutoBehavior { FallbackOnWriteError }

//    public class DBConnection
//    {
//        [SlamIgnore]
//        public void Dispose() { }
//        [SlamIgnore]
//        public string ConnectionString { get; set; }
//    }

//    public class SqlProfiler
//    {
//        [SlamIgnore]
//        public void ExecuteStart(SqlCommand Cmd, ExecuteType type) { }
//        [SlamIgnore]
//        public void ExecuteFinish(SqlCommand Cmd, ExecuteType type) { }
//    }

//    #endregion

//    public class DBConnection
//    {

//        private DBConnection PrepareConnection()
//        {
//            DBConnection ConnHelper = null;


//            return ConnHelper;
//        }
//    }

//    #region INGORES

//    public class DBCommand
//    {
//        protected enum DBConnectionUse : byte
//        {
//            Read = 1,
//            Write = 2
//        }

//        [Flags]
//        internal enum DatabaseAccessAutoBehavior : int
//        {
//            None = 0,
//            FallbackOnReadError = 1,
//            FallbackOnWriteError = 2,
//            FallbackOnEmptyRead = 4,
//            ForceFallback = 8
//        }

//        [SlamIgnore]
//        internal Hobbisoft.Slam.Core.Tracing.SqlProfiler sqlProfiler;

//        [SlamIgnore]
//        SqlCommand Cmd = new SqlCommand();

//        [SlamIgnore]
//        bool RetryExec(SqlException e, int RetryCount) { return true; }

//        [SlamIgnore]
//        void PrepareConnectionForRetry(ref DBConnection con) { }

//        [SlamIgnore]
//        DBConnection PrepareConnectionForFallback() { return new DBConnection(); }

//        [SlamIgnore]
//        void LogSQLException(SqlCommand cmd, SqlException e, DBConnection ConnHelper) { }

//        [SlamIgnore]
//        void AutoRollbackTransaction() { }

//        [SlamIgnore]
//        internal bool AllowAutoFallback(DatabaseAccessAutoBehavior behave) { return true; }

//        [SlamIgnore]
//        protected DBConnection PrepareConnection(DBConnectionUse use) { return new DBConnection(); }

//    #endregion

//    public int ExecuteNonQuery()
//    {

//        sqlProfiler.ExecuteStart(Cmd, Hobbisoft.Slam.Core.Tracing.ExecuteType.NonQuery);

//        try
//        {
//            int RetryCount = 0;
//            // If a ConnHelper is passed back, we are using local instance of
//            // the connection helper and it will be disposed below.
//            DBConnection ConnHelper = PrepareConnection(DBConnectionUse.Write);
//            DefaultInjectors.UpdateDBWatcher(ConnHelper.ConnectionString, Cmd as SqlCommand);
//            try
//            {
//                while (true)
//                {
//                    try
//                    {
//                        return Cmd.ExecuteNonQuery();
//                    }
//                    catch (SqlException e)
//                    {
//                        if (RetryExec(e, RetryCount++))
//                        {
//                            PrepareConnectionForRetry(ref ConnHelper);
//                            continue;
//                        }
//                        if (AllowAutoFallback(DatabaseAccessAutoBehavior.FallbackOnWriteError))
//                        {
//                            if (ConnHelper != null)
//                            {
//                                try { ConnHelper.Dispose(); }
//                                catch { }
//                            }
//                            ConnHelper = PrepareConnectionForFallback();
//                            RetryCount = 0;
//                            continue;
//                        }

//                        // CR 21023. Log Sql Exception
//                        LogSQLException(Cmd, e, ConnHelper);

//                        throw;
//                    }
//                }
//            }
//            finally
//            {
//                sqlProfiler.ExecuteFinish(Cmd, Hobbisoft.Slam.Core.Tracing.ExecuteType.NonQuery);
//                if (ConnHelper != null)
//                {
//                    Cmd.Connection = null;
//                    ConnHelper.Dispose();
//                }
//            }
//        }
//        catch
//        {
//            AutoRollbackTransaction();
//            throw;
//        }
//    }


//}
