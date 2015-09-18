// Camilo Galiana
// LinqToVso.PCL
// ProjectRequestProcessor.cs
// 18 / 07 / 2015

using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using LinqToVso.Linqify;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace LinqToVso
{
    /// <summary>
    ///     handles query processing for Projects
    /// </summary>
    public class ProjectRequestProcessor<T> : IRequestProcessor<T> where T : class
    {
        /// <summary>
        ///     base url for request
        /// </summary>
        public virtual string BaseUrl { get; set; }

        public IList<CustomApiParameter> CustomParameters { get; set; }

        /// <summary>
        ///     extracts parameters from lambda
        /// </summary>
        /// <param name="lambdaExpression">lambda expression with where clause</param>
        /// <returns>dictionary of parameter name/value pairs</returns>
        public virtual Dictionary<string, string> GetParameters(LambdaExpression lambdaExpression)
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

        /// <summary>
        ///     builds url based on input parameters
        /// </summary>
        /// <param name="parameters">criteria for url segments and parameters</param>
        /// <param name="expressionParameters"></param>
        /// <returns>URL conforming to VSO API</returns>
        public virtual Request BuildUrl(Dictionary<string, string> expressionParameters)
        {
            if (expressionParameters.ContainsKey("Id"))
            {
                return this.GetTeamProjectDetailsUrl(expressionParameters);
            }

            return this.GetTeamProjectsUrl(expressionParameters);
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

        private Request GetTeamProjectsUrl(Dictionary<string, string> expressionParameters)
        {
            // Gerenic call
            var req = new Request(this.BaseUrl + "/_apis/projects");
            var urlParams = req.RequestParameters;

            if (expressionParameters.ContainsKey("State"))
            {
                var state = (ProjectState) (int.Parse(expressionParameters["State"]));
                urlParams.Add(new QueryParameter("stateFilter", state.ToString()));
            }

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

        private Request GetTeamProjectDetailsUrl(Dictionary<string, string> expressionParameters)
        {
            var id = expressionParameters["Id"];

            var url = string.Format("{0}{1}{2}", this.BaseUrl, "/_apis/projects/", id);
            var req = new Request(url);
            var urlParams = req.RequestParameters;

            if (this.CustomParameters != null && this.CustomParameters.Any(x => x.Key == "Capabilities"))
            {
                urlParams.Add(new QueryParameter("includeCapabilites", "true"));
            }

            urlParams.Add(new QueryParameter("api-version", "1.0"));
            return req;
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