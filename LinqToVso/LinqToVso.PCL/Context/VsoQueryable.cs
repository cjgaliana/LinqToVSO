using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;

namespace LinqToVso.PCL.Context
{
    /// <summary>
    /// IQueryable of T part of LINQ to Vso
    /// </summary>
    /// <typeparam name="T">Type to operate on</typeparam>
    public class VsoQueryable<T> : IOrderedQueryable<T>
    {
        /// <summary>
        /// init with VsoContext
        /// </summary>
        /// <param name="context"></param>
        public VsoQueryable(VsoContext context)
        {
            Provider = new VsoQueryProvider();
            Expression = Expression.Constant(this);

            // lets provider reach back to VsoContext,
            // where execute implementation resides
            ((VsoQueryProvider)Provider).Context = context;
        }

        /// <summary>
        /// modified as internal because LINQ to Vso is Unusable
        /// without VsoContext, but provider still needs access
        /// </summary>
        /// <param name="provider">IQueryProvider</param>
        /// <param name="expression">Expression Tree</param>
        internal VsoQueryable(
            VsoQueryProvider provider,
            Expression expression)
        {
            if (provider == null)
            {
                throw new ArgumentNullException("provider");
            }

            if (expression == null)
            {
                throw new ArgumentNullException("expression");
            }

            if (!typeof(IQueryable<T>).GetTypeInfo().IsAssignableFrom(expression.Type.GetTypeInfo()))
            {
                throw new ArgumentOutOfRangeException("expression");
            }

            Provider = provider;
            Expression = expression;
        }

        /// <summary>
        /// IQueryProvider part of LINQ to Vso
        /// </summary>
        public IQueryProvider Provider { get; private set; }

        /// <summary>
        /// expression tree
        /// </summary>
        public Expression Expression { get; private set; }


        public void IncludeQueryParameter(string includeParameter)
        {
            this.IncludeParameter = includeParameter;
        }

        public string IncludeParameter { get; private set; }

        /// <summary>
        /// type of T in IQueryable of T
        /// </summary>
        public Type ElementType
        {
            get { return typeof(T); }
        }

        /// <summary>
        /// executes when iterating over collection
        /// </summary>
        /// <returns>query results</returns>
        public IEnumerator<T> GetEnumerator()
        {
            var tsk = Task.Run(() => (((VsoQueryProvider)Provider).ExecuteAsync<IEnumerable<T>>(Expression)));
            return ((IEnumerable<T>)tsk.Result).GetEnumerator();
        }

        /// <summary>
        /// non-generic execution when collection is iterated over
        /// </summary>
        /// <returns>query results</returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return (Provider.Execute<IEnumerable>(Expression)).GetEnumerator();
        }
    }
}