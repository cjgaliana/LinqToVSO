using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using LinqToVso.Linqify;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace LinqToVso
{
    public class SubscriptionRequestProcessor<T> : VsoBaseProcessor<T> where T : class
    {
    

        /// <summary>
        ///     extracts parameters from lambda
        /// </summary>
        /// <param name="lambdaExpression">lambda expression with where clause</param>
        /// <returns>dictionary of parameter name/value pairs</returns>
        public override Dictionary<string, string> GetParameters(LambdaExpression lambdaExpression)
        {
            return
                new ParameterFinder<Subscription>(
                    lambdaExpression.Body,
                    new List<string>
                    {
                        "Id" //If this parameter exists, gets the info for the given ID
                    })
                    .Parameters;
        }

        /// <summary>
        ///     builds url based on input parameters
        /// </summary>
        /// <param name="parameters">criteria for url segments and parameters</param>
        /// <param name="expressionParameters"></param>
        /// <returns>URL conforming to VSO API</returns>
        public override Request BuildUrl(Dictionary<string, string> expressionParameters)
        {
            if (expressionParameters.ContainsKey("Id"))
            {
                return this.GetSubscriptionDetailsUrl(expressionParameters);
            }

            return this.GetSubscriptionsUrl(expressionParameters);
        }

        public override List<T> ProcessResults(string vsoResponse)
        {
            var json = JObject.Parse(vsoResponse);

            if (this.IsSingleSubscriptionDetailsResponse(json))
            {
                return this.ProccessSinlgeResult(vsoResponse);
            }

            var serverData = json["value"].Children().ToList();
            return serverData.Select(item => JsonConvert.DeserializeObject<T>(item.ToString())).ToList();
        }

        private Request GetSubscriptionDetailsUrl(Dictionary<string, string> expressionParameters)
        {
            var id = expressionParameters["Id"];

            var url = string.Format("{0}{1}{2}", this.BaseUrl, "/hooks/subscriptions/", id);
            var req = new Request(url);
            var urlParams = req.RequestParameters;

            urlParams.Add(new QueryParameter("api-version", "1.0"));
            return req;
        }

        private Request GetSubscriptionsUrl(Dictionary<string, string> expressionParameters)
        {
            // Gerenic call
            var req = new Request(this.BaseUrl + "/hooks/subscriptions/");
            var urlParams = req.RequestParameters;

            urlParams.Add(new QueryParameter("api-version", "1.0"));
            return req;
        }

        private List<T> ProccessSinlgeResult(string vsoResponse)
        {
            var item = JsonConvert.DeserializeObject<T>(vsoResponse);
            return new List<T> {item};
        }

        private bool IsSingleSubscriptionDetailsResponse(JObject json)
        {
            JToken token = null;
            json.TryGetValue("value", out token);

            return token == null;
        }
    }
}