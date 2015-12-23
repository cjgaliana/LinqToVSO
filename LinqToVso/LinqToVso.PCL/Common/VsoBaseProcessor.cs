using LinqToVso.Linqify;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace LinqToVso
{
    public class VsoBaseProcessor<T> : IRequestProcessor<T> where T : class
    {
        /// <summary>
        ///     base url for request
        /// </summary>
        public virtual string BaseUrl { get; set; }

        /// <summary>
        /// base ApiVersion for request
        /// </summary>
        public virtual string ApiVersion { get; set; }

        public virtual IList<CustomApiParameter> CustomParameters { get; set; }

        /// <summary>
        ///     extracts parameters from lambda
        /// </summary>
        /// <param name="lambdaExpression">lambda expression with where clause</param>
        /// <returns>dictionary of parameter name/value pairs</returns>
        public virtual Dictionary<string, string> GetParameters(LambdaExpression lambdaExpression)
        {
            throw new NotImplementedException("You are calling the base implementation of the request processor. You need to override this method with the specific logic for this request");
        }

        /// <summary>
        ///     builds url based on input parameters
        /// </summary>
        /// <param name="expressionParameters">criteria for url segments and parameters</param>
        /// <returns>URL conforming to VSO API</returns>
        public virtual Request BuildUrl(Dictionary<string, string> expressionParameters)
        {
            throw new NotImplementedException("You are calling the base implementation of the request processor. You need to override this method with the specific logic for this request");
        }

        public virtual List<T> ProcessResults(string apiResponse)
        {
            throw new NotImplementedException("You are calling the base implementation of the request processor. You need to override this method with the specific logic for this request");
        }

        /// <summary>
        /// Gets if the json response is a "single" item or a list of items, given the VSO response format
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        public virtual bool IsSingleItemDetailsResponse(JObject json)
        {
            JToken token = null;
            json.TryGetValue("value", out token);

            return token == null;
        }

        /// <summary>
        /// Deserialize a single item response
        /// </summary>
        /// <param name="vsoResponse"></param>
        /// <returns></returns>

        public virtual List<T> ProccessSingleItemResult(string vsoResponse)
        {
            var item = JsonConvert.DeserializeObject<T>(vsoResponse);
            return new List<T> { item };
        }
    }
}