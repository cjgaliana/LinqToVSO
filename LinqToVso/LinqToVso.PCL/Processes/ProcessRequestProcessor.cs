using LinqToVso.Extensions;
using LinqToVso.Linqify;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace LinqToVso
{
    public class ProcessRequestProcessor<T> : VsoBaseProcessor<T> where T : class
    {
        public override Dictionary<string, string> GetParameters(LambdaExpression lambdaExpression)
        {
            return
                new ParameterFinder<Process>(
                    lambdaExpression.Body,
                    new List<string>
                    {
                        "Id" //If this parameter exists, gets the info for the given ID
                    })
                    .Parameters;
        }

        public override Request BuildUrl(Dictionary<string, string> expressionParameters)
        {
            if (expressionParameters.ContainsKey("Id"))
            {
                return this.BuildDetailUrl(expressionParameters);
            }

            var url = Utilities.CombineUrls(this.BaseUrl, "process", "processes");

            var req = new Request(url);
            req.AddApiVersionParameter(this.ApiVersion);

            return req;
        }

        public override List<T> ProcessResults(string vsoResponse)
        {
            var json = JObject.Parse(vsoResponse);

            if (this.IsSingleItemDetailsResponse(json))
            {
                return this.ProccessSingleItemResult(vsoResponse);
            }

            var serverData = json["value"].Children().ToList();

            var resultList = serverData.Select(data => JsonConvert.DeserializeObject<Process>(data.ToString())).ToList();

            return resultList.OfType<T>().ToList();
        }

        private Request BuildDetailUrl(Dictionary<string, string> expressionParameters)
        {
            var id = expressionParameters["Id"];

            var url = Utilities.CombineUrls(this.BaseUrl,
                "process",
                "processes",
                id);

            var req = new Request(url);
            req.AddApiVersionParameter(this.ApiVersion);
            return req;
        }
    }
}