using Linqify;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace LinqToVso.PCL.Hooks
{
    public class HookRequestProcessor<T> : IRequestProcessor<T> where T : class
    {
        private HookType _hookType;

        /// <summary>
        ///     base url for request
        /// </summary>
        public string BaseUrl { get; set; }

        public IList<CustomApiParameter> CustomParameters { get; set; }

        /// <summary>
        ///     extracts parameters from lambda
        /// </summary>
        /// <param name="lambdaExpression">lambda expression with where clause</param>
        /// <returns>dictionary of parameter name/value pairs</returns>
        public virtual Dictionary<string, string> GetParameters(LambdaExpression lambdaExpression)
        {
            return
                new ParameterFinder<Hook>(
                    lambdaExpression.Body,
                    new List<string>
                    {
                        "Type", //Consumer/Publisher
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
            if (!expressionParameters.ContainsKey("Type"))
            {
                throw new ArgumentException("A Hook type is required to perform the operation");
            }

            var type = expressionParameters["Type"];

            this._hookType = (HookType)Enum.Parse(typeof(HookType), type);
            switch (this._hookType)
            {
                case HookType.Publisher:
                    return this.BuildPublisherUrl(expressionParameters);

                case HookType.Consumer:
                    return this.BuildConsumerUrl(expressionParameters);

                default:
                    throw new ArgumentOutOfRangeException("type", "Hook Type not valid");
            }
        }

        private Request BuildConsumerUrl(Dictionary<string, string> expressionParameters)
        {
            var url = string.Format("{0}/{1}/{2}",
                this.BaseUrl,
                "_apis/hooks",
                "consumers");

            var req = new Request(url);
            IList<QueryParameter> urlParams = req.RequestParameters;

            urlParams.Add(new QueryParameter("api-version", "1.0"));
            return req;
        }

        private Request BuildPublisherUrl(Dictionary<string, string> expressionParameters)
        {
            var url = string.Format("{0}/{1}/{2}",
               this.BaseUrl,
               "_apis/hooks",
               "publishers");

            var req = new Request(url);
            IList<QueryParameter> urlParams = req.RequestParameters;

            urlParams.Add(new QueryParameter("api-version", "1.0"));
            return req;
        }

        public List<T> ProcessResults(string vsoResponse)
        {
            switch (this._hookType)
            {
                case HookType.Publisher:
                    return this.ProccessPublisherResult(vsoResponse);

                case HookType.Consumer:
                    return this.ProccessConsumerResult(vsoResponse);

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private List<T> ProccessPublisherResult(string vsoResponse)
        {
            JObject json = JObject.Parse(vsoResponse);

            if (this.IsSingleHookDetailsResponse(json))
            {
                return this.ProccessSinlgeResult(vsoResponse);
            }

            List<JToken> serverData = json["value"].Children().ToList();

            var resultList = new List<Hook>();

            foreach (JToken data in serverData)
            {
                var item = JsonConvert.DeserializeObject<Hook>(data.ToString());
                item.Type = this._hookType;
                resultList.Add(item);
            }

            return resultList.OfType<T>().ToList();
        }

        private bool IsSingleHookDetailsResponse(JObject json)
        {
            JToken token = null;
            json.TryGetValue("value", out token);

            return token == null;
        }

        private List<T> ProccessConsumerResult(string vsoResponse)
        {
            JObject json = JObject.Parse(vsoResponse);

            if (this.IsSingleHookDetailsResponse(json))
            {
                return this.ProccessSinlgeResult(vsoResponse);
            }

            List<JToken> serverData = json["value"].Children().ToList();

            var resultList = new List<Hook>();

            foreach (JToken data in serverData)
            {
                var item = JsonConvert.DeserializeObject<Hook>(data.ToString());
                item.Type = this._hookType;
                resultList.Add(item);
            }

            return resultList.OfType<T>().ToList();
        }

        private List<T> ProccessSinlgeResult(string vsoResponse)
        {
            var item = JsonConvert.DeserializeObject<T>(vsoResponse);
            return new List<T> { item };
        }
    }
}