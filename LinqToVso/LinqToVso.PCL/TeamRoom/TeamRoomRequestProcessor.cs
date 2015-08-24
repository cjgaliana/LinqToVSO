using Linqify;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace LinqToVso.PCL.TeamRoom
{
    public class TeamRoomRequestProcessor<T> : IRequestProcessor<T> where T : class
    {
        public string BaseUrl { get; set; }
        public IList<CustomApiParameter> CustomParameters { get; set; }

        public Dictionary<string, string> GetParameters(LambdaExpression lambdaExpression)
        {
            return
               new ParameterFinder<TeamRoom>(
                   lambdaExpression.Body,
                   new List<string>
                    {
                        "Id", //If this parameter exists, gets the info for the given ID
                    })
                   .Parameters;
        }

        /// <summary>
        /// builds url based on input parameters
        /// </summary>
        /// <param name="parameters">criteria for url segments and parameters</param>
        /// <param name="expressionParameters"></param>
        /// <returns>URL conforming to VSO API</returns>
        public virtual Request BuildUrl(Dictionary<string, string> expressionParameters)
        {
            if (expressionParameters.ContainsKey("Id"))
            {
                return this.GetTeamRoomDetailsUrl(expressionParameters);
            }

            return this.GetTeamRoomsUrl(expressionParameters);
        }

        private Request GetTeamRoomsUrl(Dictionary<string, string> expressionParameters)
        {
            // Gerenic call
            var req = new Request(this.BaseUrl + "/_apis/chat/rooms");
            var urlParams = req.RequestParameters;

            urlParams.Add(new QueryParameter("api-version", "1.0"));
            return req;
        }

        private Request GetTeamRoomDetailsUrl(Dictionary<string, string> expressionParameters)
        {
            var id = expressionParameters["Id"];

            var url = string.Format("{0}{1}{2}", this.BaseUrl, "/_apis/chat/rooms/{0}", id);
            var req = new Request(url);
            var urlParams = req.RequestParameters;

            urlParams.Add(new QueryParameter("api-version", "1.0"));
            return req;
        }

        public List<T> ProcessResults(string vsoResponse)
        {
            var json = JObject.Parse(vsoResponse);

            if (this.IsSingleProjectDetailsResponse(json))
            {
                return this.ProccessSinlgeResult(vsoResponse);
            }

            var serverData = json["value"].Children().ToList();
            return serverData.Select(item => JsonConvert.DeserializeObject<T>(item.ToString())).ToList();
        }

        private List<T> ProccessSinlgeResult(string vsoResponse)
        {
            var item = JsonConvert.DeserializeObject<T>(vsoResponse);
            return new List<T> { item };
        }

        private bool IsSingleProjectDetailsResponse(JObject json)
        {
            JToken token = null;
            json.TryGetValue("value", out token);

            return token == null;
        }
    }
}