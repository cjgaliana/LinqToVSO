using Linqify;
using LinqToVso.PCL.Factories;

namespace LinqToVso
{
    public partial class VsoContext : LinqifyContext
    {
        #region Header Constants

        private readonly string DefaultUserAgent = "LINQ-To-VSO/3.0";

        #endregion Header Constants

        #region Constants

        private readonly string BaseUrlFormat = "https://{0}.visualstudio.com/DefaultCollection";
        private readonly string DefaultApiVersion = "1.0";

        #endregion Constants

        private string _apiVersion;

        public VsoContext(ILinqifyExecutor executor)
            : base(executor)
        {
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
            IRequestProcessor<T> req = VsoRequestProcessorFactory.Create<T>(requestType);

            if (this.BaseUrl != null)
            {
                req.BaseUrl = this.BaseUrl;
            }

            req.CustomParameters = this._customParameters;

            return req;
        }
    }
}