using LinqToVso.Linqify;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace LinqToVso
{
    public class HookRequestProcessor<T> : VsoBaseProcessor<T> where T : class
    {
        private HookType _hookType;

        /// <summary>
        ///     extracts parameters from lambda
        /// </summary>
        /// <param name="lambdaExpression">lambda expression with where clause</param>
        /// <returns>dictionary of parameter name/value pairs</returns>
        public override Dictionary<string, string> GetParameters(LambdaExpression lambdaExpression)
        {
            return
                new ParameterFinder<Hook>(
                    lambdaExpression.Body,
                    new List<string>
                    {
                        "Type" //Consumer/Publisher
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

        public override List<T> ProcessResults(string vsoResponse)
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

        private Request BuildConsumerUrl(Dictionary<string, string> expressionParameters)
        {
            var url = Utilities.CombineUrls(this.BaseUrl, "hooks", "consumers");

            var req = new Request(url);
            var urlParams = req.RequestParameters;

            urlParams.Add(new QueryParameter("api-version", "1.0"));
            return req;
        }

        private Request BuildPublisherUrl(Dictionary<string, string> expressionParameters)
        {
            var url = Utilities.CombineUrls(this.BaseUrl, "hooks", "publishers");

            var req = new Request(url);
            var urlParams = req.RequestParameters;

            urlParams.Add(new QueryParameter("api-version", "1.0"));
            return req;
        }

        private List<T> ProccessPublisherResult(string vsoResponse)
        {
            var json = JObject.Parse(vsoResponse);

            if (this.IsSingleItemDetailsResponse(json))
            {
                return this.ProccessSingleItemResult(vsoResponse);
            }

            var serverData = json["value"].Children().ToList();

            var resultList = new List<Hook>();

            foreach (var data in serverData)
            {
                var item = JsonConvert.DeserializeObject<Hook>(data.ToString());
                item.Type = this._hookType;
                resultList.Add(item);
            }

            return resultList.OfType<T>().ToList();
        }

        private List<T> ProccessConsumerResult(string vsoResponse)
        {
            var json = JObject.Parse(vsoResponse);

            if (this.IsSingleItemDetailsResponse(json))
            {
                return this.ProccessSingleItemResult(vsoResponse);
            }

            var serverData = json["value"].Children().ToList();

            var resultList = new List<Hook>();

            foreach (var data in serverData)
            {
                var item = JsonConvert.DeserializeObject<Hook>(data.ToString());
                item.Type = this._hookType;
                resultList.Add(item);
            }

            return resultList.OfType<T>().ToList();
        }
    }
}