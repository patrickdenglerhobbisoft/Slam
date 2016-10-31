using System;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Slam.UnitTests.Classes;
using Slam.DynamicInjection;
using System.Diagnostics;


namespace Slam.Test
{

    [TestClass]
    public class BasicTests
    {

        [TestMethod]
        public void SlamAll()
        {
            SlamStatic();
            SlamInstance();
            SlamPartialClassReplacement();
            SlamOverloaded();
            SlamIgnore();
            SlamSealed();
            SlamSquirt();
            SlamUI();
        }


        [TestMethod]
        public void SlamStatic()
        {
            Log.Output(@"Current Value of MySourceClass.PublicStaticWhatsTheTime() = " + MySourceClass.PublicStaticWhatsTheTime());

            Injector.SlamClass(typeof(MySourceClass), typeof(MyMockClass));
            Log.Output(" New Value of MySourceClass.PublicStaticWhatsTheTime() = " + MySourceClass.PublicStaticWhatsTheTime());

        }

        [TestMethod]
        public void SlamInstance()
        {
            InstanceMethod original = new InstanceMethod();
            Log.Output(@" Current Value of IntanceMethod.Add(1,1) = " + original.Add(1, 1).ToString());

            Injector.SlamClass(typeof(InstanceMethod), typeof(InstanceMethod_NewMath));
            Log.Output(" New Value of IntanceMethod.Add(1,1) = " + original.Add(1, 1).ToString());
        }


        [TestMethod]
        public void SlamPartialClassReplacement()
        {
            //  given a class with 3 functions
            Log.Output(@"Current Value of BigClass.FunctionAs,FunctionBs,FunctionCs = " + BigClass.FunctionAs() + BigClass.FunctionBs() + BigClass.FunctionCs());

            //  replace 2 of them
            Injector.SlamClass(typeof(BigClass), typeof(BigClass_PartialSlam));

            Log.Output("New Value of BigClass.FunctionAs,FunctionBs,FunctionCs = " + BigClass.FunctionAs() + BigClass.FunctionBs() + BigClass.FunctionCs());

        }

        [TestMethod]
        public void SlamOverloaded()
        {
            //  given a class with 3 overloaded functions
            ClassWithOverloadedFunctions overloadedFunctions = new ClassWithOverloadedFunctions();


            var intOutput = overloadedFunctions.GetString(1);
            var floatOutput = overloadedFunctions.GetString(1.1f);
            var stringOutput = overloadedFunctions.GetString("One");

            Log.Output(@"Current Value of ClassWithOverloadedFunctions \n"
                       + "\tint : " + intOutput + "\n"
                       + "\tfloat : " + floatOutput + "\n"
                       + "\tstring : " + stringOutput + "\n");
            //  replace 2 of them
            Injector.SlamClass(typeof(ClassWithOverloadedFunctions), typeof(ClassWithOverloadedFunctions_Slam));

            intOutput = overloadedFunctions.GetString(1);
            floatOutput = overloadedFunctions.GetString(1.1f);
            stringOutput = overloadedFunctions.GetString("One");

            Log.Output(@"New Value of ClassWithOverloadedFunctions \n"
                       + "\tint : " + intOutput + "\n"
                       + "\tfloat : " + floatOutput + "\n"
                       + "\tstring : " + stringOutput + "\n");
        }


        [TestMethod]
        public void SlamIgnore()
        {
            Log.Output(@"Current Value of ComplicatedDatabaseClass.FunctionCall() = " + ComplicatedDatabaseClass.FunctionCall(new ComplicatedDatabaseClass.ReferencedTypeUsedInComplicatedDatabaseClass()));

            Injector.SlamClass(typeof(ComplicatedDatabaseClass), typeof(ComplicatedDatabaseClass_Slam));
            Log.Output(" New Value of ComplicatedDatabaseClass.FunctionCall() = " + ComplicatedDatabaseClass.FunctionCall(new ComplicatedDatabaseClass.ReferencedTypeUsedInComplicatedDatabaseClass()));

        }

        [TestMethod]
        public void SlamSealed()
        {
            // Below, we can't create a new SfSessionEntryFactory because the class is an interface based internal (sealed)
            // Try uncommenting the lines to see 

            //ExactTarget.DynamicInjection.UnitTests.Classes.SfSessionEntryFactory sf =
            //    new ExactTarget.DynamicInjection.UnitTests.Classes.SfSessionEntryFactory();

            // and so is the static function we want to call

            //ExactTarget.DynamicInjection.UnitTests.Classes.SfSessionEntryFactory.GetSessionEntry(true,
            //    "Wish I could use you in my unit tests :(");


            // object will be the result of the function call (see more below)
            object result = new object();
            var methodParameterData = new object[] { true, "I'm calling a sealed method!" };

            // with a small adjustment to injector, we can pass this method in to be slammed/bonded/injected
            MethodInfo methodInfo = InjectionHelper.GetSealedMethod(@"D:\inetpub\Common\ExactTarget.DynamicInjection.UnitTests.Classes.dll",
                "ExactTarget.DynamicInjection.UnitTests.Classes.SfSessionEntryFactory", "GetSessionEntry", methodParameterData,

                // but if we pass true to InvokeMethod and ref Object, it will call the method and return the result to your object
                true, ref result);

            // if result were typed you could simply cast ie
            //SFSessionEntry typedResult = (SFSessionEntry)result;

            Log.Output("Result of Slamning sealed method is " + Convert.ToBoolean(result).ToString());

        }

    
        [TestMethod]
        public void SlamSquirt()
        {
            // Insert (Squirt) One (or more) line's into an existing function
            Squirtee.MyFunction();
            Injector.SlamClass(typeof(Squirtee), typeof(Squirter));
            Squirtee.MyFunction();

        }
    
        [TestMethod]
        public void SlamUI()
        {
                DefaultInjectors.LogMessage("Goodbye World");

        }


        #region In Development
        //[TestMethod]
        //public void SlamUI2()
        //{
        //    //int MID = 10650712;

        //    //AccountContext.SetCurrent(MID, MID, 0);
        //    //var principal = AccountContext.Current.ToETPrincipal();
        //    //List<SqlParameter> Params = new List<SqlParameter>();

        //    //var p = new SqlParameter("@CustomerId", SqlDbType.Int);
        //    //p.SqlValue = 1;
        //    //Params.Add(p);
        //    //SqlParameter[] parms = Params.ToArray<SqlParameter>();

        //    //Injector.SlamClass(typeof(ExactTarget.Core.DBCommand), typeof(ExactTarget.DynamicInjection.DBCommand));
        //    //ExactTarget.Core.DBConnection.ExecuteNonQuery(principal, DatabaseType.Member, "dbo.Dev_Testing", parms, 300, 0);
        //}

        ////
        ////        [TestMethod]
        ////
        //[HandleProcessCorruptedStateExceptions]
        //public void SlamExternal(int instantiate)
        //{
        //    ExternalClass ec = null;
        //    ExternalClass_Slam ec_slam = null;
        //    string results = string.Empty;
        //    if (instantiate == 0) // neither
        //    {

        //    }
        //    else if (instantiate == 1) // both
        //    {
        //        ec = new ExternalClass();
        //        ec_slam = new ExternalClass_Slam();
        //    }
        //    else if (instantiate == 2)
        //    {
        //        // source
        //        ec = new ExternalClass();
        //    }
        //    else if (instantiate == 3)
        //    {
        //        // target
        //        ec_slam = new ExternalClass_Slam();
        //    }
        //    Injector.SlamClass(typeof(ExternalClass), typeof(ExternalClass_Slam));

        //    if (ec == null)
        //        ec = new ExternalClass();

        //    if (ec_slam == null)
        //        ec_slam = new ExternalClass_Slam();

        //    try
        //    {
        //        results = ec.ImExternal();
        //    }
        //    catch (Exception e)
        //    {


        //        Log.Output(instantiate.ToString() + " : " + e.Message);
        //    }


        //    Log.Output(instantiate.ToString() + " : " + results);


        //}

        #endregion

    }




}
