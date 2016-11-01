using Slam.DynamicInjection;
using System;
using System.Data;
using System.Data.SqlClient;



namespace Slam.UnitTests.Classes
{

    #region Simple Static

    public class MySourceClass
    {
        public static string WhatsTheTIme() { return DateTime.Now.ToShortTimeString(); }

    }
    public class MySourceClass_Slam
    {
        public static string WhatsTheTIme() { return "It's time to get Ill!!"; }

    }

    #endregion





















    //public class MyMockClass
    //{
    //    public static string PublicStaticWhatsTheTime_Slam() { return "It's Time to Get Ill (Public Static)"; }

    //}



    #region InstanceMethod


    public class InstanceMethod
    {
        public int Add(int a, int b) { return a + b; }
    }


    public class InstanceMethod_NewMath
    {
        public int Add(int a, int b) { return a + b - 100; }
    }

    #endregion

    #region Partial


    public class BigClass
    {
        public class ReferencedClass
        {
            public static string GetValueFromSomething()
            {
                return "value";
            }
        }

        public static string OverloadedFunction(int i)
        {
            Console.WriteLine(ReferencedClass.GetValueFromSomething());
            return "aaaaaaaaaaaaaa";
        }

        public static string OverloadedFunction(string s) { return "bbbbbbbbbbbbbb"; }
        public static string OverloadedFunction(float f) { return "cccccccccccccc"; }
    }


    public class BigClass_PartialSlam
    {
        // I want to inject into overload int and overload string, but not overload float
        // AND
        // I want to preserve the basic functionality of overloaded int, but i just want to return a different value


        // in order for this class to understand a referenced class exists, i need to add that referenced class
        // to this class, and that function, but I DO NOT want them injected.

        // by default, every method that is in your slam class is replaced (and that means, you can replace 1- max methods / properties in a class
        // if you don't supply the method, it won't be replaced

        // in order to avoid injection, I add an attribute SlamIgnore
        // the referenced ignored classes, methods, functions, etc, needn't implement any
        // functionality, they need only to match signatures
        [SlamIgnore]
        public class ReferencedClass
        {
            [SlamIgnore]
            public static string GetValueFromSomething()
            {
                return "whatever";
            }
        }

        // now I can change the return value and still preserve the original method functionality
        public static string OverloadedFunction(int i)
        {
            Console.WriteLine(ReferencedClass.GetValueFromSomething());
            return "XXXXXXXXXXXXXX";
        }

        // and while this will reperesent injecting and overloaded function (or constructor)
        // and so we will return something different as well
        public static string OverloadedFunction(string s) { return "YYYYYYYYYYYYY"; }

        // this one I am just not including, so it will remain the same
       // public static string OverloadedFunction(float f) { return "cccccccccccccc"; }

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
