// Guids.cs
// MUST match guids.h
using System;

namespace Slam.Tools
{
    static class GuidList
    {
        public const string guidHobbisoft_Slam_DynamicInjection_ToolsPkgString = "aa4ff18a-2477-4c86-a0e4-ef8f4d11e82d";
        public const string guidHobbisoft_Slam_DynamicInjection_ToolsCmdSetString = "6d955fc8-0da6-46a6-b541-b801e3b96ef7";
        public const string guidToolWindowPersistanceString = "f368e4bc-785b-4272-a352-f312d8491b1f";

        public static readonly Guid guidHobbisoft_Slam_DynamicInjection_ToolsCmdSet = new Guid(guidHobbisoft_Slam_DynamicInjection_ToolsCmdSetString);
    };
}