// Camilo Galiana
// LinqToVso.PCL
// IncludeClause.cs
// 25 / 07 / 2015

using LinqToVso.PCL.Context;
using System.Linq;

namespace LinqToVso
{
    public static class IncludeClause
    {
        public static IQueryable<T> Include<T>(this IQueryable<T> query, string parameterName)
        {
            var vsoQuery = query as VsoQueryable<T>;
            if (vsoQuery == null)
            {
                return query;
            }

            vsoQuery.IncludeQueryParameter(parameterName);
            return vsoQuery;
        }
    }
}