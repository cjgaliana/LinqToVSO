using LinqToVso.Linqify;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace LinqToVso
{
    public partial class VsoContext : LinqifyContext
    {
        private string _apiVersion;

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
            get { return string.Format(VsoConstans.VSO_BASE_UR_FORMAT, this.Account); }
        }

        public string ApiVersion
        {
            get
            {
                return string.IsNullOrWhiteSpace(this._apiVersion)
                    ? VsoConstans.VSO_DEFAULT_API_VERSION
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
                    : VsoConstans.DEFAULT_USER_AGENT;
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

        protected override IRequestProcessor<T> CreateRequestProcessor<T>(Type requestType)
        {
            var req = VsoRequestProcessorFactory.Create<T>(requestType);

            if (!string.IsNullOrWhiteSpace(this.BaseUrl))
            {
                req.BaseUrl = this.BaseUrl;
            }

            if (!string.IsNullOrWhiteSpace(this.ApiVersion))
            {
                req.ApiVersion = this.ApiVersion;
            }

            req.CustomParameters = this._customParameters;

            return req;
        }

        public override Task<object> ExecuteAsync<T>(
            Expression expression, bool isEnumerable, IList<CustomApiParameter> customParameters = null)
        {
            return base.ExecuteAsync<T>(expression, isEnumerable, customParameters);
        }
    }
}