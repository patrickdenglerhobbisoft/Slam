using Slam.DynamicInjection;
using System;
using System.Data;
using System.Data.SqlClient;



namespace Slam.UnitTests.Classes
{

    #region Simple Static

    public class MySourceClass
    {
        public string MyProperty { get; set; }
        public static string PublicStaticWhatsTheTime() { return DateTime.Now.ToShortTimeString(); }
        public string WhatsTheTimeCaller() { return WhatsTheTime(); }
        private string WhatsTheTime() { return DateTime.Now.ToShortTimeString(); }
    }


    public class MyMockClass
    {
        public string MyProperty_Slam { get; set; }
        public static string PublicStaticWhatsTheTime_Slam() { return "It's Time to Get Ill (Public Static)"; }
        private string WhatsTheTime_Slam() { return "It's Time to Get Ill (private Instance)"; }
    }

    #endregion

    #region InstanceMethod


    public class InstanceMethod
    {
        public int Add(int a, int b) { return a + b; }
    }


    public class InstanceMethod_NewMath
    {
        public int Add(int a, int b) { return a + b -100; }
    }

    #endregion

    #region Partial

    public class BigClass
    {
        public static string FunctionAs() { return  "aaaaaaaaaaaaaa"; }
        public static string FunctionBs() { return  "bbbbbbbbbbbbbb"; }
        public static string FunctionCs() { return  "cccccccccccccc"; }
    }


    public class BigClass_PartialSlam
    {
        public static string FunctionAs() { return "AAAAAAAAAAAAAAA"; }
        public static string FunctionCs() { return "CCCCCCCCCCCCCCC";}
       
    }

    #endregion

    #region ClassWithOverloadedFunctions

    public class ClassWithOverloadedFunctions
    {
        public string GetString(int Number) { return Number.ToString(); }
        public string GetString(string Number) { return Number; }
        public string GetString(float Number) { return Number.ToString(); }
    }


    public class ClassWithOverloadedFunctions_Slam
    {
        public string GetString(int Number) { return "Slammed int parameter"; }
        public string GetString(string Number) { return "Slammed string paramater"; }
    }

    #endregion

    #region  SlamIgnore

    public class ComplicatedDatabaseClass
    {
        public static string FunctionCall(ReferencedTypeUsedInComplicatedDatabaseClass input)
        {
            return ("We want to replace this function");
        }
        public class ReferencedTypeUsedInComplicatedDatabaseClass
        {
            public string ReturnValue { get; set; }
            public ReferencedTypeUsedInComplicatedDatabaseClass()
            {
                // assume this one is quite complicated, perhaps like DBCommand
            }
        }
    }

    public class ComplicatedDatabaseClass_Slam
    {
        public static string FunctionCall(ReferencedTypeUsedInComplicatedDatabaseClass input)
        {
            return ("This will replace the FunctionCall method..");
        }

        [SlamIgnore]
        public class ReferencedTypeUsedInComplicatedDatabaseClass
        {
            // we need to add the type so we can compile the function, but we 
            // do NOT want to inject these

             [SlamIgnore]
            public string ReturnValue { get; set; }
             [SlamIgnore]
            public ReferencedTypeUsedInComplicatedDatabaseClass()
            {
                // assume this one is quite complicated, perhaps like DBCommand
            }
        }


        public class ConnHelper
        {
           
        }
        //public partial class DBConnection
        //{
        //    public interface  IAccountContext
        //    {
                
        //    }

        //    public class DatabaseType
        //    {
                
        //    }
        //    public DBConnection(IAccountContext context, DatabaseType dbType)
        //    {
                
        //    }

        //    public class DBCommand
        //    {
                
        //    }
        //    public static DBConnection GetDBCommand(IAccountContext context, DatabaseType dbtype)
        //    {
        //        return new DBConnection(context,dbtype);
        //    }
        //    public static SqlDataReader ExecuteReader(IAccountContext accountContext, DatabaseType dbType,
        //        string commandText, SqlParameter[] parameterArray,
        //        CommandType commandType, int commandTimeOut, byte retryLimit)
        //    {
        //        //return ExecuteReader(accountContext, dbType, commandText, parameterArray,
        //        //    commandType, commandTimeOut, retryLimit, DBAccessBehavior.Default);
        //        using (DBConnection ConnHelper = new DBConnection(accountContext, dbType))
        //        {
        //            using (DBCommand CmdHelper = ConnHelper.GetDBCommand(
        //                commandText, commandType, commandTimeOut, retryLimit))
        //            {
        //                AddParameters(parameterArray, CmdHelper.Parameters);
        //                DefaultInjectors.UpdateDBWatcher(ConnHelper.connectionString, CmdHelper.Cmd);
        //                return CmdHelper.ExecuteReader(CommandBehavior.CloseConnection);
        //            }
        //        }
        //    }
        //}

    }

   
    #endregion
}
