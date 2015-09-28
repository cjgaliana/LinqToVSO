// Camilo Galiana

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace LinqToVso.Linqify
{
    public abstract class LinqifyContext : IDisposable
    {
        protected IList<CustomApiParameter> _customParameters;

        protected LinqifyContext(ILinqifyExecutor executor)
        {
            if (executor == null)
            {
                throw new ArgumentNullException("executor", "The API executor cannot be null");
            }
            this.Executor = executor;
        }

        /// <summary>
        ///     This contains the JSON string from the API response to the most recent query.
        /// </summary>
        public string RawResult { get; protected set; }

        public ILinqifyExecutor Executor { get; protected set; }

        /// <summary>
        ///     Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        ///     Releases unmanaged and - optionally - managed resources
        /// </summary>
        /// <param name="disposing">
        ///     <c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only
        ///     unmanaged resources.
        /// </param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                var disposableExecutor = this.Executor as IDisposable;
                if (disposableExecutor != null)
                {
                    disposableExecutor.Dispose();
                }
            }
        }

        /// <summary>
        ///     Called by QueryProvider to execute queries
        /// </summary>
        /// <param name="expression">ExpressionTree to parse</param>
        /// <param name="isEnumerable">Indicates whether expression is enumerable</param>
        /// <param name="customParameters">Custom API parameters passed from the Linq query</param>
        /// <returns>list of objects with query results</returns>
        public virtual async Task<object> ExecuteAsync<T>(Expression expression, bool isEnumerable,
            IList<CustomApiParameter> customParameters = null)
            where T : class
        {
            this._customParameters = customParameters;

            // request processor is specific to request type (i.e. Status, User, etc.)
            var reqProc = this.CreateRequestProcessor<T>(expression);

            // get input parameters that go on the REST query URL
            var parameters = GetRequestParameters(expression, reqProc);

            // construct REST endpoint, based on input parameters
            var request = reqProc.BuildUrl(parameters);

            string results;

            //process request through the REST API

            results = await this.Executor.QueryApiAsync(request, reqProc).ConfigureAwait(false);

            this.RawResult = results;

            // Transform results into objects
            var queryableList = reqProc.ProcessResults(results);

            // Copy the IEnumerable entities to an IQueryable.
            var queryableItems = queryableList.AsQueryable();

            // Copy the expression tree that was passed in, changing only the first
            // argument of the innermost MethodCallExpression.
            // -- Transforms IQueryable<T> into List<T>, which is (IEnumerable<T>)
            var treeCopier = new ExpressionTreeModifier<T>(queryableItems);
            var newExpressionTree = treeCopier.CopyAndModify(expression);

            // This step creates an IQueryable that executes by replacing Queryable methods with Enumerable methods.
            if (isEnumerable)
            {
                var data = queryableItems.Provider.CreateQuery(newExpressionTree);
                return data;
            }

            return queryableItems.Provider.Execute<object>(newExpressionTree);
        }

        /// <summary>
        ///     Search the where clause for query parameters
        /// </summary>
        /// <param name="expression">Input query expression tree</param>
        /// <param name="reqProc">Processor specific to this request type</param>
        /// <returns>Name/value pairs of query parameters</returns>
        private static Dictionary<string, string> GetRequestParameters<T>(Expression expression,
            IRequestProcessor<T> reqProc)
        {
            var parameters = new Dictionary<string, string>();

            // WHERE CLAUSE
            var whereExpressions = new WhereClauseFinder().GetAllWheres(expression);
            foreach (var whereExpression in whereExpressions)
            {
                var lambdaExpression = (LambdaExpression) ((UnaryExpression) (whereExpression.Arguments[1])).Operand;

                // translate variable references in expression into constants
                lambdaExpression = (LambdaExpression) Evaluator.PartialEval(lambdaExpression);

                var newParameters = reqProc.GetParameters(lambdaExpression);
                foreach (var newParameter in newParameters)
                {
                    if (!parameters.ContainsKey(newParameter.Key))
                    {
                        parameters.Add(newParameter.Key, newParameter.Value);
                    }
                }
            }

            // TAKE CLAUSE
            var takeExpressions = new TakeClauseFinder().GetAllTakes(expression);
            foreach (var takeExpression in takeExpressions)
            {
                parameters.Add(TakeClauseFinder.TakeMethodName, takeExpression.Arguments[1].ToString());
            }

            // SKIP CLAUSE
            var skipExpressions = new SkipClauseFinder().GetAllSkips(expression);
            foreach (var skipExpression in skipExpressions)
            {
                parameters.Add(SkipClauseFinder.SkipMethodName, skipExpression.Arguments[1].ToString());
            }

            return parameters;
        }

        protected internal virtual IRequestProcessor<T> CreateRequestProcessor<T>()
            where T : class
        {
            var requestType = typeof (T).Name;

            var req = this.CreateRequestProcessor<T>(requestType);

            return req;
        }

        /// <summary>
        ///     TestMethodory method for returning a request processor
        /// </summary>
        /// <typeparam name="T">type of request</typeparam>
        /// <returns>request processor matching type parameter</returns>
        internal IRequestProcessor<T> CreateRequestProcessor<T>(Expression expression)
            where T : class
        {
            if (expression == null)
            {
                throw new ArgumentNullException("expression",
                    "Expression passed to CreateRequestProcessor must not be null.");
            }

            var requestType = new MethodCallExpressionTypeFinder().GetGenericType(expression).Name;

            var req = this.CreateRequestProcessor<T>(requestType);
            return req;
        }

        protected abstract IRequestProcessor<T> CreateRequestProcessor<T>(string requestType) where T : class;
    }
}