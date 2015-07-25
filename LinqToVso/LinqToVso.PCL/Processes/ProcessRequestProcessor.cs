using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace LinqToVso.PCL.Processes
{
    public class ProcessRequestProcessor<T> : IRequestProcessor<T> where T : class
    {
        /// <summary>
        ///     base url for request
        /// </summary>
        public string BaseUrl { get; set; }

        public string ExtraParameters { get; set; }

        /// <summary>
        ///     extracts parameters from lambda
        /// </summary>
        /// <param name="lambdaExpression">lambda expression with where clause</param>
        /// <returns>dictionary of parameter name/value pairs</returns>
        public virtual Dictionary<string, string> GetParameters(LambdaExpression lambdaExpression)
        {
            return
                new ParameterFinder<Process>(
                    lambdaExpression.Body,
                    new List<string>
                    {
                        "Id", //If this parameter exists, gets the info for the given ID
                    })
                    .Parameters;
        }

        /// <summary>
        ///     builds url based on input parameters
        /// </summary>
        /// <param name="parameters">criteria for url segments and parameters</param>
        /// <param name="expressionParameters"></param>
        /// <returns>URL conforming to VSO API</returns>
        public virtual Request BuildUrl(Dictionary<string, string> expressionParameters)
        {
            if (expressionParameters.ContainsKey("Id"))
            {
                return this.BuildDetailUrl(expressionParameters);
            }

            var url = string.Format("{0}/{1}/{2}",
                this.BaseUrl,
                "_apis/process",
                "processes");

            var req = new Request(url);
            IList<QueryParameter> urlParams = req.RequestParameters;

            urlParams.Add(new QueryParameter("api-version", "1.0"));
            return req;
        }

        private Request BuildDetailUrl(Dictionary<string, string> expressionParameters)
        {
            var id = expressionParameters["Id"];
            var url = string.Format("{0}/{1}/{2}/{3}",
                this.BaseUrl,
                "_apis/process",
                "processes",
                id);

            var req = new Request(url);
            IList<QueryParameter> urlParams = req.RequestParameters;

            urlParams.Add(new QueryParameter("api-version", "1.0"));
            return req;
        }

        public List<T> ProcessResults(string vsoResponse)
        {
            JObject json = JObject.Parse(vsoResponse);

            if (this.IsSingleResult(json))
            {
                return this.ProcessSingleResult(vsoResponse);
            }

            List<JToken> serverData = json["value"].Children().ToList();

            var resultList = serverData.Select(data => JsonConvert.DeserializeObject<Process>(data.ToString())).ToList();

            return resultList.OfType<T>().ToList();
        }

        private List<T> ProcessSingleResult(string vsoResponse)
        {
            var item = JsonConvert.DeserializeObject<T>(vsoResponse);
            return new List<T>() { item };
        }

        private bool IsSingleResult(JObject json)
        {
            JToken token = null;
            json.TryGetValue("value", out token);

            return token == null;
        }
    }
}