using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using LinqToVso.Linqify;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace LinqToVso.PCL.Team
{
    internal enum NemberRequestType
    {
        Team,
        TeamRoom
    }

    public class TeamMemberRequestProcessor<T> : IRequestProcessor<T> where T : class
    {
        private string _projectId;
        private NemberRequestType _requestInfoType;
        private string _teamId;
        private string _teamRoomId;

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
                new ParameterFinder<TeamMember>(
                    lambdaExpression.Body,
                    new List<string>
                    {
                        "Id", //If this parameter exists, gets the info for the given ID
                        "ProjectId", //The parent project
                        "TeamId", // The parent team,
                        "TeamRoomId", // The team room id (in case you are searching for the team room users)
                        TakeClauseFinder.TakeMethodName, //Number of team projects to return.
                        SkipClauseFinder.SkipMethodName //Number of team projects to skip
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
            if (expressionParameters.ContainsKey("TeamRoomId"))
            {
                return this.BuildTeamRoomMembersBuild(expressionParameters);
            }

            if (expressionParameters.ContainsKey("ProjectId") && expressionParameters.ContainsKey("TeamId"))
            {
                return this.BuildTeamMembersBuild(expressionParameters);
            }

            throw new ArgumentException("A TeamRoomId  or ProjectId+TeamID are required to perform this operation");
        }

        public List<T> ProcessResults(string vsoResponse)
        {
            switch (this._requestInfoType)
            {
                case NemberRequestType.Team:
                    return this.ParseTeamMemberResults(vsoResponse);
                    break;

                case NemberRequestType.TeamRoom:
                    return this.ParseTeamRoomMemberResults(vsoResponse);
                    break;

                default:
                    //throw new ArgumentOutOfRangeException("Its not possible parse the Member Info results.");
                    throw new ArgumentOutOfRangeException("RequestInfoType",
                        "It's not possible parse the response because the deserializer info is needed");
            }
        }

        private Request BuildTeamRoomMembersBuild(Dictionary<string, string> expressionParameters)
        {
            this._teamRoomId = expressionParameters["TeamRoomId"];
            this._requestInfoType = NemberRequestType.TeamRoom;

            var url = string.Format("{0}/{1}/{2}/{3}",
                this.BaseUrl,
                "_apis/rooms",
                this._teamRoomId,
                "users");

            var req = new Request(url);
            var urlParams = req.RequestParameters;

            urlParams.Add(new QueryParameter("api-version", "1.0"));
            return req;
        }

        private Request BuildTeamMembersBuild(Dictionary<string, string> expressionParameters)
        {
            this._projectId = expressionParameters["ProjectId"];
            this._teamId = expressionParameters["TeamId"];

            this._requestInfoType = NemberRequestType.Team;

            var url = string.Format("{0}/{1}/{2}/{3}/{4}/{5}/",
                this.BaseUrl,
                "_apis/projects",
                this._projectId,
                "teams",
                this._teamId,
                "members");

            var req = new Request(url);
            var urlParams = req.RequestParameters;

            if (expressionParameters.ContainsKey(TakeClauseFinder.TakeMethodName))
            {
                urlParams.Add(new QueryParameter("$top", expressionParameters[TakeClauseFinder.TakeMethodName]));
            }

            if (expressionParameters.ContainsKey(SkipClauseFinder.SkipMethodName))
            {
                urlParams.Add(new QueryParameter("$skip", expressionParameters[SkipClauseFinder.SkipMethodName]));
            }

            urlParams.Add(new QueryParameter("api-version", "1.0"));
            return req;
        }

        public List<T> ParseTeamMemberResults(string vsoResponse)
        {
            var json = JObject.Parse(vsoResponse);
            var serverData = json["value"].Children().ToList();

            var resultList = new List<TeamMember>();

            foreach (var data in serverData)
            {
                var item = JsonConvert.DeserializeObject<TeamMember>(data.ToString());
                item.ProjectId = this._projectId;
                item.TeamId = this._teamId;

                resultList.Add(item);
            }

            return resultList.OfType<T>().ToList();
        }

        public List<T> ParseTeamRoomMemberResults(string vsoResponse)
        {
            //JObject json = JObject.Parse(vsoResponse);

            var json = JObject.Parse(vsoResponse);

            if (this.IsSingleProjectDetailsResponse(json))
            {
                return this.ProccessSinlgeResult(vsoResponse);
            }

            // Parse multiple users
            var serverData = json["value"].Children().ToList();

            var resultList = new List<TeamMember>();

            foreach (var data in serverData)
            {
                var item = JsonConvert.DeserializeObject<TeamMember>(data.ToString());

                if (!string.IsNullOrWhiteSpace(this._teamRoomId))
                {
                    var roomId = 0;
                    int.TryParse(this._teamRoomId, out roomId);
                    item.TeamRoomId = roomId;
                }

                resultList.Add(item);
            }

            return resultList.OfType<T>().ToList();
        }

        private List<T> ProccessSinlgeResult(string vsoResponse)
        {
            var item = JsonConvert.DeserializeObject<T>(vsoResponse);
            return new List<T> {item};
        }

        private bool IsSingleProjectDetailsResponse(JObject json)
        {
            JToken token = null;
            json.TryGetValue("value", out token);

            return token == null;
        }
    }
}