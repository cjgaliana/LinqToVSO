using LinqToVso.Common;
using LinqToVso.PCL.Authorization;
using LinqToVso.PCL.Common;
using LinqToVso.PCL.Context;
using LinqToVso.PCL.Hooks;
using LinqToVso.PCL.Processes;
using LinqToVso.PCL.Team;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace LinqToVso
{
    public partial class VsoContext : IDisposable
    {
        private readonly IAuthorizer _authorizer;
        private string _apiVersion;

        #region Header Constants

        private readonly string DefaultUserAgent = "LINQ-To-VSO/3.0";

        #endregion Header Constants

        #region Constants

        private readonly string BaseUrlFormat = "https://{0}.visualstudio.com/DefaultCollection";
        private readonly string DefaultApiVersion = "1.0";

        #endregion Constants

        public VsoContext(IAuthorizer authorizer)
        {
            if (authorizer == null)
            {
                throw new ArgumentNullException("authorizer", "VSO authorizer is required");
            }

            this._authorizer = authorizer;
            this.VsoExecutor = new VsoExecute(authorizer);
        }

        /// <summary>
        ///     http://{account}.visualstudio.com
        /// </summary>
        public string Account
        {
            get
            {
                if (this._authorizer != null)
                {
                    return this._authorizer.Account;
                }
                return string.Empty;
            }
        }

        public string BaseUrl
        {
            get { return string.Format(this.BaseUrlFormat, this.Account); }
        }

        public string ApiVersion
        {
            get
            {
                return string.IsNullOrWhiteSpace(this._apiVersion)
                    ? this.DefaultApiVersion
                    : this._apiVersion;
            }
            set { this._apiVersion = value; }
        }

        /// <summary>
        ///     Gets and sets HTTP UserAgent header
        /// </summary>
        public string UserAgent
        {
            get
            {
                return this.VsoExecutor != null
                    ? this.VsoExecutor.UserAgent
                    : this.DefaultUserAgent;
            }
            set
            {
                if (this.VsoExecutor != null)
                {
                    this.VsoExecutor.UserAgent = value;
                }
            }
        }

        /// <summary>
        ///     Methods for communicating with VSO.
        /// </summary>
        internal IVsoExecute VsoExecutor { get; set; }

        /// <summary>
        ///     Response headers from Twitter Queries
        /// </summary>
        public IDictionary<string, string> ResponseHeaders
        {
            get
            {
                if (this.VsoExecutor != null)
                {
                    return this.VsoExecutor.ResponseHeaders;
                }
                return null;
            }
        }

        /// <summary>
        ///     This contains the JSON string from the VSO response to the most recent query.
        /// </summary>
        public string RawResult { get; set; }

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
                var disposableExecutor = this.VsoExecutor as IDisposable;
                if (disposableExecutor != null)
                {
                    disposableExecutor.Dispose();
                }
            }
        }

        /*******************************************************************************************************/

        private IList<string> _includeParameters;

        /// <summary>
        ///     Called by QueryProvider to execute queries
        /// </summary>
        /// <param name="expression">ExpressionTree to parse</param>
        /// <param name="isEnumerable">Indicates whether expression is enumerable</param>
        /// <param name="includeParameters"></param>
        /// <returns>list of objects with query results</returns>
        public virtual async Task<object> ExecuteAsync<T>(Expression expression, bool isEnumerable, IList<string> includeParameters = null)
            where T : class
        {
            this._includeParameters = includeParameters;
            // request processor is specific to request type (i.e. Status, User, etc.)
            IRequestProcessor<T> reqProc = CreateRequestProcessor<T>(expression);

            // get input parameters that go on the REST query URL
            Dictionary<string, string> parameters = GetRequestParameters(expression, reqProc);

            // construct REST endpoint, based on input parameters
            Request request = reqProc.BuildUrl(parameters);

            string results;

            //process request through VSO

            results = await this.VsoExecutor.QueryVsoAsync(request, reqProc).ConfigureAwait(false);

            this.RawResult = results;

            // Transform results into objects
            List<T> queryableList = reqProc.ProcessResults(results);

            // Copy the IEnumerable entities to an IQueryable.
            IQueryable<T> queryableItems = queryableList.AsQueryable();

            // Copy the expression tree that was passed in, changing only the first
            // argument of the innermost MethodCallExpression.
            // -- Transforms IQueryable<T> into List<T>, which is (IEnumerable<T>)
            var treeCopier = new ExpressionTreeModifier<T>(queryableItems);
            Expression newExpressionTree = treeCopier.CopyAndModify(expression);

            // This step creates an IQueryable that executes by replacing Queryable methods with Enumerable methods.
            if (isEnumerable)
            {
                IQueryable data = queryableItems.Provider.CreateQuery(newExpressionTree);
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
            MethodCallExpression[] whereExpressions = new WhereClauseFinder().GetAllWheres(expression);
            foreach (MethodCallExpression whereExpression in whereExpressions)
            {
                var lambdaExpression = (LambdaExpression)((UnaryExpression)(whereExpression.Arguments[1])).Operand;

                // translate variable references in expression into constants
                lambdaExpression = (LambdaExpression)Evaluator.PartialEval(lambdaExpression);

                Dictionary<string, string> newParameters = reqProc.GetParameters(lambdaExpression);
                foreach (var newParameter in newParameters)
                {
                    if (!parameters.ContainsKey(newParameter.Key))
                    {
                        parameters.Add(newParameter.Key, newParameter.Value);
                    }
                }
            }

            // TAKE CLAUSE
            MethodCallExpression[] takeExpressions = new TakeClauseFinder().GetAllTakes(expression);
            foreach (MethodCallExpression takeExpression in takeExpressions)
            {
                parameters.Add(TakeClauseFinder.TakeMethodName, takeExpression.Arguments[1].ToString());
            }

            // SKIP CLAUSE
            MethodCallExpression[] skipExpressions = new SkipClauseFinder().GetAllSkips(expression);
            foreach (MethodCallExpression skipExpression in skipExpressions)
            {
                parameters.Add(SkipClauseFinder.SkipMethodName, skipExpression.Arguments[1].ToString());
            }

            return parameters;
        }

        protected internal virtual IRequestProcessor<T> CreateRequestProcessor<T>()
            where T : class
        {
            string requestType = typeof(T).Name;

            IRequestProcessor<T> req = CreateRequestProcessor<T>(requestType);

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

            string requestType = new MethodCallExpressionTypeFinder().GetGenericType(expression).Name;

            IRequestProcessor<T> req = CreateRequestProcessor<T>(requestType);
            return req;
        }

        protected internal IRequestProcessor<T> CreateRequestProcessor<T>(string requestType)
            where T : class
        {
            string baseUrl = this.BaseUrl;
            IRequestProcessor<T> req;

            switch (requestType)
            {
                case "Project":
                    req = new ProjectRequestProcessor<T>();
                    break;

                case "Team":
                    req = new TeamRequestProcessor<T>();
                    break;

                case "TeamMember":
                    req = new TeamMemberRequestProcessor<T>();
                    break;

                case "Process":
                    req = new ProcessRequestProcessor<T>();
                    break;

                case "Hook":
                    req = new HookRequestProcessor<T>();
                    break;

                default:
                    throw new ArgumentException("Type, " + requestType + " isn't a supported LINQ to VSO entity.",
                        "requestType");
            }

            if (baseUrl != null)
            {
                req.BaseUrl = baseUrl;
                req.IncludeParameters = _includeParameters;
            }

            return req;
        }
    }
}