using System;
using Linqify;
using LinqToVso.PCL.Hooks;
using LinqToVso.PCL.Processes;
using LinqToVso.PCL.Team;

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

            if (this.BaseUrl != null)
            {
                req.BaseUrl = this.BaseUrl;
            }

            req.CustomParameters = this._customParameters;

            return req;
        }
    }
}