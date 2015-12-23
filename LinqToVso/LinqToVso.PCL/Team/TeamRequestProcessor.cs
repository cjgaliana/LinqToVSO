// Camilo Galiana
// LinqToVso.PCL
// TeamRequestProcessor.cs
// 19 / 07 / 2015

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
    public class TeamRequestProcessor<T> : VsoBaseProcessor<T> where T : class
    {
        private string _projectId;

        public override Dictionary<string, string> GetParameters(LambdaExpression lambdaExpression)
        {
            return
                new ParameterFinder<Team>(
                    lambdaExpression.Body,
                    new List<string>
                    {
                        //"account",   //Your Visual Studio Online account.
                        //"api-version", //Version of the API to use.
                        TakeClauseFinder.TakeMethodName, //Number of team projects to return.
                        SkipClauseFinder.SkipMethodName, //Number of team projects to skip
                        "Id", //If this parameter exists, gets the info for the given ID
                        "ProjectId" //The parent project
                    })
                    .Parameters;
        }

        public override Request BuildUrl(Dictionary<string, string> expressionParameters)
        {
            if (!expressionParameters.ContainsKey("ProjectId"))
            {
                throw new ArgumentException("A project ID is required to perform this operation");
            }

            if (expressionParameters.ContainsKey("Id"))
            {
                return this.GetTeamDetailsUrl(expressionParameters);
            }

            this._projectId = expressionParameters["ProjectId"];

            var url = Utilities.CombineUrls(
                this.BaseUrl,
                "projects",
                this._projectId,
                "teams");
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

        public override List<T> ProcessResults(string vsoResponse)
        {
            var json = JObject.Parse(vsoResponse);

            if (this.IsSingleItemDetailsResponse(json))
            {
                return this.ProccessSingleItemResult(vsoResponse);
            }

            var serverData = json["value"].Children().ToList();

            var resultList = new List<Team>();

            foreach (var data in serverData)
            {
                var item = JsonConvert.DeserializeObject<Team>(data.ToString());
                item.ProjectId = this._projectId;
                resultList.Add(item);
            }

            return resultList.OfType<T>().ToList();
        }

        private Request GetTeamDetailsUrl(Dictionary<string, string> expressionParameters)
        {
            var projectId = expressionParameters["ProjectId"];
            var teamId = expressionParameters["Id"];

            var url = Utilities.CombineUrls(
                this.BaseUrl,
                "projects",
                projectId,
                "teams",
                teamId);

            var req = new Request(url);
            req.AddApiVersionParameter(this.ApiVersion);
            return req;
        }

        public override List<T> ProccessSingleItemResult(string vsoResponse)
        {
            var item = JsonConvert.DeserializeObject<Team>(vsoResponse);
            item.ProjectId = this._projectId;

            var list = new List<Team> { item };
            return list.OfType<T>().ToList();
        }
    }
}