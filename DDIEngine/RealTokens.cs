﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;


public static class RealTokens
{
    public static Dictionary<MethodBase, int> Token_MethodBase;
    public static Dictionary<Type, int> Token_Type;
    public static Dictionary<FieldInfo, int> Token_FieldInfo;    

    static RealTokens()
    {
        Addition.module = typeof(RealTokens).Module;        
        var L = IniListType(AOP.TokenGetCurrentMethod, Addition.module.ResolveMethod, Addition.module.ResolveField);
        Token_MethodBase = L[0].ConvertKey<MethodBase>();
        Token_FieldInfo = L[1].ConvertKey<FieldInfo>();
        L = IniListType(AOP.TokenInt32, Addition.module.ResolveType);
        Token_Type = L[0].ConvertKey<Type>();
    }

    public static int GetRealToken(Type T)
    {
        int N = 0;
        return (Token_Type.TryGetValue(T, out N)) ? N : T.MetadataToken;
    }

    public static int GetRealToken(MethodBase T)
    {
        int N = 0;
        /*var GetErr=typeof(MethodBase).GetMethod("get_FullName",Addition.AALL);
        var FullName=Token_MethodBase.Keys.Select(x =>GetErr.Invoke(x,null).ToString()+"     ==" +x.FullTypeName()+"  -- "+x.Name);
        System.Diagnostics.Debug.WriteLine(FullName.Count.ToString());*/
        return (Token_MethodBase.TryGetValue(T, out N)) ? N : T.MetadataToken;
    }

    public static int GetRealToken(FieldInfo T)
    {
        int N = 0;
        return (Token_FieldInfo.TryGetValue(T, out N)) ? N : T.MetadataToken;
    }

    public delegate object GetDataToken(int i);
    public static List<Dictionary<object, int>> IniListType(int i, params GetDataToken[] t)
    {
        var Res = new List<Dictionary<object, int>>();
        foreach (var T in t) Res.Add(new Dictionary<object, int>());
        int k;
        int j = i;
        Action ChangeEnum = () => j--;
        Action Select = () =>
        {
            while (true)
            {

                for (k = 0; k < t.Length; k++)
                {
                    try
                    {
                        Res[k].Add(t[k](j), j);
                    }
                    catch
                    {
                        continue;
                    }
                    break;
                }
                if (k == t.Length)
                {
                    try
                    {
                        Addition.module.ResolveSignature(j);
                    }
                    catch
                    {
                        try
                        {
                            Addition.module.ResolveString(j);
                        }
                        catch
                        {
                            break;
                        }
                    }
                }
                ChangeEnum();
            }
        };

        Select();
        j = i + 1;
        ChangeEnum = () => j++;
        Select();

        return Res;
    }
}

public static class AdditToken
{
    public static Dictionary<T, int> ConvertKey<T>(this Dictionary<object, int> Me)
    {
        Dictionary<T, int> Ret = new Dictionary<T, int>();
        foreach (var Pair in Me) Ret.Add((T)Pair.Key, Pair.Value);
        return Ret;
    }
    
    public static List<byte> RealTokenL(this MemberInfo Me)
    {
        if ((Me is MethodInfo) || (Me is ConstructorInfo))
            return Addition.FromInt32List(RealTokens.GetRealToken(Me as MethodBase));
        else if (Me is FieldInfo)
            return Addition.FromInt32List(RealTokens.GetRealToken(Me as FieldInfo));
        else if (Me is Type)
            return Addition.FromInt32List(RealTokens.GetRealToken(Me as Type));
        else return null;
    }

    public static int RealToken(this MemberInfo Me)
    {
        if ((Me is MethodInfo) || (Me is ConstructorInfo))
            return RealTokens.GetRealToken(Me as MethodBase);
        else if (Me is FieldInfo)
            return RealTokens.GetRealToken(Me as FieldInfo);
        else if (Me is Type)
            return RealTokens.GetRealToken(Me as Type);
        else return 0;
    }

   /*public static int RealToken(this MethodBase Me)
     {
         return RealTokens.GetRealToken(Me);
     }
     public static List<byte> RealTokenL(this MethodBase Me)
     {
         return AdditAOP.FromInt32List(Me.RealToken());
     }
     public static int RealToken(this Type Me)
     {
         return RealTokens.GetRealToken(Me);
     }
     public static List<byte> RealTokenL(this Type Me)
     {
         return AdditAOP.FromInt32List(Me.RealToken());
     }
     public static int RealToken(this FieldInfo Me)
     {
         return RealTokens.GetRealToken(Me);
     }
     public static List<byte> RealTokenL(this FieldInfo Me)
     {
        return AdditAOP.FromInt32List(Me.RealToken());
     }*/
}