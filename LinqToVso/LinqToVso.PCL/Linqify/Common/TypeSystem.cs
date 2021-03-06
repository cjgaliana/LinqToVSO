﻿/***********************************************************
 * Credits:
 *
 * MSDN Documentation -
 * Walkthrough: Creating an IQueryable LINQ Provider
 *
 * http://msdn.microsoft.com/en-us/library/bb546158.aspx
 *
 * Matt Warren's Blog -
 * LINQ: Building an IQueryable Provider:
 *
 * http://blogs.msdn.com/mattwar/default.aspx
 * *********************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace LinqToVso.Linqify
{
    internal static class TypeSystem
    {
        //#if NETFX_CORE
        internal static Type GetElementType(Type seqType)
        {
            var ienum = FindIEnumerable(seqType);
            if (ienum == null) return seqType;
            {
                return ienum.GenericTypeArguments[0];
            }
        }

        private static Type FindIEnumerable(Type seqType)
        {
            var seqTypeInfo = seqType.GetTypeInfo();
            if (seqType == null || seqType == typeof (string))
            {
                return null;
            }

            if (seqTypeInfo.IsArray)
            {
                return typeof (IEnumerable<>).MakeGenericType(seqTypeInfo.GetElementType());
            }

            if (seqTypeInfo.IsGenericType)
            {
                foreach (var arg in seqTypeInfo.GenericTypeArguments)
                {
                    var ienum = typeof (IEnumerable<>).MakeGenericType(arg);
                    if (ienum.GetTypeInfo().IsAssignableFrom(seqTypeInfo))
                    {
                        return ienum;
                    }
                }
            }

            var ifaces = seqTypeInfo.ImplementedInterfaces.ToArray();
            if (ifaces != null && ifaces.Length > 0)
            {
                foreach (var iface in ifaces)
                {
                    var ienum = FindIEnumerable(iface);
                    if (ienum != null) return ienum;
                }
            }

            if (seqTypeInfo.BaseType != null && seqTypeInfo.BaseType != typeof (object))
            {
                return FindIEnumerable(seqTypeInfo.BaseType);
            }

            return null;
        }
    }
}