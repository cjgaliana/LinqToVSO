using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using LinqToVso.Linqify;
using LinqToVso.PCL.Authorization;
using LinqToVso.PCL.Context;
using LinqToVso.PCL.Factories;

namespace LinqToVso
{
    public partial class VsoContext : LinqifyContext
    {
        private string _apiVersion;

        #region Header Constants

        private readonly string DefaultUserAgent = "LINQ-To-VSO/1.0";

        #endregion Header Constants

        #region Constants

        private readonly string BaseUrlFormat = "https://{0}.visualstudio.com/DefaultCollection";
        private readonly string DefaultApiVersion = "1.0";

        #endregion Constants

        public VsoContext(string account, string oauthToken)
            : this(account, new VsoExecute(new OauthHttpClientHandler(oauthToken)))
        {
        }

        public VsoContext(string account, string user, string password)
            : this(account, new VsoExecute(new BasicAuthHttpClientHandler(user, password)))
        {
        }

        public VsoContext(string account, ILinqifyExecutor executor)
            : base(executor)
        {
            this.Account = account;
        }

        /// <summary>
        ///     http://{account}.visualstudio.com
        /// </summary>
        public string Account { get; set; }

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
        ///     Methods for communicating with VSO API.
        /// </summary>
        internal ILinqifyExecutor VsoExecutor { get; set; }

        protected override IRequestProcessor<T> CreateRequestProcessor<T>(string requestType)
        {
            var req = VsoRequestProcessorFactory.Create<T>(requestType);

            if (this.BaseUrl != null)
            {
                req.BaseUrl = this.BaseUrl;
            }

            req.CustomParameters = this._customParameters;

            return req;
        }

        public override Task<object> ExecuteAsync<T>(Expression expression, bool isEnumerable,
            IList<CustomApiParameter> customParameters = null)
        {
            return base.ExecuteAsync<T>(expression, isEnumerable, customParameters);
        }


    }
}