using System.Collections.Generic;

namespace LinqToVso.Linqify
{
    public class Request
    {
        public Request(string endpoint)
        {
            this.Endpoint = endpoint;
            this.RequestParameters = new List<QueryParameter>();
        }

        public string Endpoint { get; set; }
        public IList<QueryParameter> RequestParameters { get; internal set; }

        public string QueryString
        {
            get { return Utilities.BuildQueryString(this.RequestParameters); }
        }

        public void AddParameter(string name, string value)
        {
            if (this.RequestParameters == null)
            {
                this.RequestParameters = new List<QueryParameter>();
            }

            this.RequestParameters.Add(new QueryParameter(name, value));
        }

        public string FullUrl
        {
            get
            {
                var queryString = this.QueryString;

                if (queryString.Length > 0)
                {
                    return this.Endpoint + "?" + this.QueryString;
                }
                return this.Endpoint;
            }
        }
    }
}