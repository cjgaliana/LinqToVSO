using LinqToVso.PCL.Context;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace LinqToVso
{
    public static class AsyncSupport
    {
        public static async Task<List<T>> ToListAsync<T>(this IQueryable<T> query)
        {
            var vsoQuery = query as VsoQueryable<T>;
            var provider = query.Provider as VsoQueryProvider;
            var extraparameters = vsoQuery.IncludeParameter;

            IEnumerable<T> results = (IEnumerable<T>)await provider.ExecuteAsync<IEnumerable<T>>(query.Expression, extraparameters).ConfigureAwait(false);

            return results.ToList();
        }

        public static async Task<T> FirstOrDefaultAsync<T>(this IQueryable<T> query)
            where T : class
        {
            var provider = query.Provider as VsoQueryProvider;

            IEnumerable<T> results = (IEnumerable<T>)await provider.ExecuteAsync<T>(query.Expression).ConfigureAwait(false);

            return results.FirstOrDefault();
        }


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

        public static async Task<T> FirstAsync<T>(this IQueryable<T> query)
            where T : class
        {
            var provider = query.Provider as VsoQueryProvider;

            IEnumerable<T> results = (IEnumerable<T>)await provider.ExecuteAsync<T>(query.Expression).ConfigureAwait(false);

            return results.First();
        }

        public static async Task<T> SingleOrDefaultAsync<T>(this IQueryable<T> query)
            where T : class
        {
            var provider = query.Provider as VsoQueryProvider;

            IEnumerable<T> results = (IEnumerable<T>)await provider.ExecuteAsync<T>(query.Expression).ConfigureAwait(false);

            return results.SingleOrDefault();
        }

        public static async Task<T> SingleAsync<T>(this IQueryable<T> query)
            where T : class
        {
            var provider = query.Provider as VsoQueryProvider;

            IEnumerable<T> results = (IEnumerable<T>)await provider.ExecuteAsync<T>(query.Expression).ConfigureAwait(false);

            return results.Single();
        }

        /// <summary>
        /// Enables use of .NET Cancellation Framework for this query.
        /// </summary>
        /// <param name="streaming">Query being extended</param>
        /// <param name="callback">Your code for handling VSO content</param>
        /// <returns>Streaming instance to support further LINQ opertations</returns>
        public static IQueryable<T> WithCancellation<T>(this IQueryable<T> query, CancellationToken cancelToken)
            where T : class
        {
            var provider = query.Provider as VsoQueryProvider;
            if (provider != null)
            {
                provider
                       .Context
                       .VsoExecutor
                       .CancellationToken = cancelToken;
            }

            return query;
        }
    }
}