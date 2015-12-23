using LinqToVso.Extensions;
using LinqToVso.Linqify;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace LinqToVso
{
    public class TeamRoomRequestProcessor<T> : VsoBaseProcessor<T> where T : class
    {
        public override Dictionary<string, string> GetParameters(LambdaExpression lambdaExpression)
        {
            return
                new ParameterFinder<TeamRoom>(
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
                return this.GetTeamRoomDetailsUrl(expressionParameters);
            }

            return this.GetTeamRoomsUrl(expressionParameters);
        }

        public override List<T> ProcessResults(string vsoResponse)
        {
            var json = JObject.Parse(vsoResponse);

            if (this.IsSingleItemDetailsResponse(json))
            {
                return this.ProccessSingleItemResult(vsoResponse);
            }

            var serverData = json["value"].Children().ToList();
            return serverData.Select(item => JsonConvert.DeserializeObject<T>(item.ToString())).ToList();
        }

        private Request GetTeamRoomsUrl(Dictionary<string, string> expressionParameters)
        {
            // Generic call
            var url = Utilities.CombineUrls(this.BaseUrl, "chat/rooms");
            var req = new Request(url);
            req.AddApiVersionParameter(this.ApiVersion);
            return req;
        }

        private Request GetTeamRoomDetailsUrl(Dictionary<string, string> expressionParameters)
        {
            var id = expressionParameters["Id"];

            var url = Utilities.CombineUrls(this.BaseUrl, "/chat/rooms/", id);

            var req = new Request(url);
            req.AddApiVersionParameter(this.ApiVersion);
            return req;
        }
    }
}