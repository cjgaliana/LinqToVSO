// Camilo Galiana
// LinqToVso.PCL
// TeamRequestProcessor.cs
// 19 / 07 / 2015

using LinqToVso.PCL.Common;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace LinqToVso.PCL.Team
{
    public class TeamRequestProcessor<T> : IRequestProcessor<T> where T : class
    {
        private string _projectId;

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
                new ParameterFinder<Team>(
                    lambdaExpression.Body,
                    new List<string>
                    {
                        //"account",   //Your Visual Studio Online account.
                        //"api-version", //Version of the API to use.
                        TakeClauseFinder.TakeMethodName, //Number of team projects to return.
                        SkipClauseFinder.SkipMethodName, //Number of team projects to skip
                        "Id", //If this parameter exists, gets the info for the given ID
                        "ProjectId", //The parent project
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
            if (!expressionParameters.ContainsKey("ProjectId"))
            {
                throw new ArgumentException("A project ID is required to perform this operation");
            }

            if (expressionParameters.ContainsKey("Id"))
            {
                return this.GetTeamDetailsUrl(expressionParameters);
            }

            this._projectId = expressionParameters["ProjectId"];

            string url = string.Format("{0}/{1}/{2}/{3}",
                this.BaseUrl,
                "_apis/projects",
                this._projectId,
                "teams");
            var req = new Request(url);
            IList<QueryParameter> urlParams = req.RequestParameters;

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

        public List<T> ProcessResults(string vsoResponse)
        {
            JObject json = JObject.Parse(vsoResponse);

            if (this.IsSingleProjectDetailsResponse(json))
            {
                return this.ProccessSinlgeResult(vsoResponse);
            }

            List<JToken> serverData = json["value"].Children().ToList();

            var resultList = new List<Team>();

            foreach (JToken data in serverData)
            {
                var item = JsonConvert.DeserializeObject<Team>(data.ToString());
                item.ProjectId = this._projectId;
                resultList.Add(item);
            }

            return resultList.OfType<T>().ToList();
        }

        private Request GetTeamDetailsUrl(Dictionary<string, string> expressionParameters)
        {
            string projectId = expressionParameters["ProjectId"];
            string teamId = expressionParameters["Id"];

            string url = string.Format("{0}/{1}/{2}/{3}/{4}",
                this.BaseUrl,
                "_apis/projects",
                projectId,
                "teams",
                teamId);

            var req = new Request(url);
            IList<QueryParameter> urlParams = req.RequestParameters;

            urlParams.Add(new QueryParameter("api-version", "1.0"));
            return req;
        }

        private List<T> ProccessSinlgeResult(string vsoResponse)
        {
            var item = JsonConvert.DeserializeObject<Team>(vsoResponse);
            item.ProjectId = this._projectId;

            var list = new List<Team> { item };
            return list.OfType<T>().ToList();
        }

        private bool IsSingleProjectDetailsResponse(JObject json)
        {
            JToken token = null;
            json.TryGetValue("value", out token);

            return token == null;
        }
    }
}