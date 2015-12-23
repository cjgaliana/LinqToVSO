// Camilo Galiana
// LinqToVso.PCL
// ProjectRequestProcessor.cs
// 18 / 07 / 2015

using LinqToVso.Extensions;
using LinqToVso.Linqify;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace LinqToVso
{
    /// <summary>
    ///     handles query processing for Projects
    /// </summary>
    public class ProjectRequestProcessor<T> : VsoBaseProcessor<T> where T : class
    {
        public override Dictionary<string, string> GetParameters(LambdaExpression lambdaExpression)
        {
            return
                new ParameterFinder<Project>(
                    lambdaExpression.Body,
                    new List<string>
                    {
                        "State", //Return team projects in a specific team project state.
                        TakeClauseFinder.TakeMethodName, //Number of team projects to return.
                        SkipClauseFinder.SkipMethodName, //Number of team projects to skip
                        "Id" //If this parameter exists, gets the info for the given ID
                    })
                    .Parameters;
        }

        public override Request BuildUrl(Dictionary<string, string> expressionParameters)
        {
            return expressionParameters.ContainsKey("Id")
                ? this.GetTeamProjectDetailsUrl(expressionParameters)
                : this.GetTeamProjectsUrl(expressionParameters);
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

        private Request GetTeamProjectsUrl(Dictionary<string, string> expressionParameters)
        {
            // Generic call
            var endpoint = Utilities.CombineUrls(this.BaseUrl, "projects");
            var req = new Request(endpoint);

            if (expressionParameters.ContainsKey("State"))
            {
                var state = (ProjectState)(int.Parse(expressionParameters["State"]));
                req.AddParameter("stateFilter", state.ToString());
            }

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

        private Request GetTeamProjectDetailsUrl(Dictionary<string, string> expressionParameters)
        {
            var id = expressionParameters["Id"];

            var endpoint = Utilities.CombineUrls(this.BaseUrl, "projects", id);
            var req = new Request(endpoint);

            if (this.CustomParameters != null && this.CustomParameters.Any(x => x.Value.ToString() == Project.CapabilitiesKey))
            {
                req.AddParameter("includeCapabilites", "true");
            }

            req.AddApiVersionParameter(this.ApiVersion);
            return req;
        }
    }
}