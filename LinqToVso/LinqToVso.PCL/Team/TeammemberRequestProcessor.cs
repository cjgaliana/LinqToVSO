using LinqToVso.Extensions;
using LinqToVso.Linqify;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace LinqToVso
{
    internal enum NemberRequestType
    {
        Team,
        TeamRoom
    }

    public class TeamMemberRequestProcessor<T> : VsoBaseProcessor<T> where T : class
    {
        private string _projectId;
        private NemberRequestType _requestInfoType;
        private string _teamId;
        private string _teamRoomId;

        public override Dictionary<string, string> GetParameters(LambdaExpression lambdaExpression)
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

        public override Request BuildUrl(Dictionary<string, string> expressionParameters)
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

        public override List<T> ProcessResults(string vsoResponse)
        {
            switch (this._requestInfoType)
            {
                case NemberRequestType.Team:
                    return this.ParseTeamMemberResults(vsoResponse);

                case NemberRequestType.TeamRoom:
                    return this.ParseTeamRoomMemberResults(vsoResponse);

                default:
                    throw new ArgumentOutOfRangeException("RequestInfoType",
                        "It's not possible parse the response because the deserializer info is needed");
            }
        }

        private Request BuildTeamRoomMembersBuild(Dictionary<string, string> expressionParameters)
        {
            this._teamRoomId = expressionParameters["TeamRoomId"];
            this._requestInfoType = NemberRequestType.TeamRoom;

            var url = Utilities.CombineUrls(this.BaseUrl,
                "rooms",
                this._teamRoomId,
                "users");

            var req = new Request(url);
            req.AddApiVersionParameter(this.ApiVersion);
            return req;
        }

        private Request BuildTeamMembersBuild(Dictionary<string, string> expressionParameters)
        {
            this._projectId = expressionParameters["ProjectId"];
            this._teamId = expressionParameters["TeamId"];

            this._requestInfoType = NemberRequestType.Team;

            var url = Utilities.CombineUrls(this.BaseUrl,
                "projects",
                this._projectId,
                "teams",
                this._teamId,
                "members");

            var req = new Request(url);

            if (expressionParameters.ContainsKey(TakeClauseFinder.TakeMethodName))
            {
                req.AddParameter("$top", expressionParameters[TakeClauseFinder.TakeMethodName]);
            }

            if (expressionParameters.ContainsKey(SkipClauseFinder.SkipMethodName))
            {
                req.AddParameter("$skip", expressionParameters[SkipClauseFinder.SkipMethodName]);
            }

            req.AddApiVersionParameter(this.ApiVersion);
            return req;
        }

        private List<T> ParseTeamMemberResults(string vsoResponse)
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

        private List<T> ParseTeamRoomMemberResults(string vsoResponse)
        {
            var json = JObject.Parse(vsoResponse);

            if (this.IsSingleItemDetailsResponse(json))
            {
                return this.ProccessSingleItemResult(vsoResponse);
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
    }
}