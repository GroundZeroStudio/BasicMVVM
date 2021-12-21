using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Utility
{
    public static bool AnyOne<T>(this T[] rArray, Func<T, bool> rPredicate)
    {
        if (rPredicate == null)
        {
            return true;
        }

        int nCount = rArray.Length;
        for (int i = 0; i < nCount; i++)
        {
            if (rPredicate.Invoke(rArray[i]))
            {
                return true;
            }
        }

        return false;
    }
    public static List<T> GetComponentsInChildrenUtilOrigin<T>(Component rComp) where T : Component
    {
        List<T> rCompResults = new List<T>();
        rCompResults.AddRange(rComp.transform.GetComponents<T>());
        int nChildCount = rComp.transform.childCount;
        for (int i = 0; i < nChildCount; i++)
        {
            var rChildTrans = rComp.transform.GetChild(i);
            GetComponentsInChildrenUtilOrigin(rCompResults, rComp.GetType(), rChildTrans);
        }
        return rCompResults;
    }

    private static void GetComponentsInChildrenUtilOrigin<T>(List<T> rComps, Type rOriginCompType, Transform rTrans) where T : Component
    {
        var rOriginComps = rTrans.GetComponents(rOriginCompType);
        if (rOriginComps.Length > 0) return;

        rComps.AddRange(rTrans.GetComponents<T>());

        int nChildCount = rTrans.childCount;
        for (int i = 0; i < nChildCount; i++)
        {
            var rChildTrans = rTrans.GetChild(i);
            GetComponentsInChildrenUtilOrigin(rComps, rOriginCompType, rChildTrans);
        }
    }

    public static void SafeExecute(Action rAction)
    {
        if (rAction != null) rAction();
    }
    public static void SafeExecute(Action<List<object>> rAction, List<object> rEventArg)
    {
        if (rAction != null) rAction(rEventArg);
    }
}
