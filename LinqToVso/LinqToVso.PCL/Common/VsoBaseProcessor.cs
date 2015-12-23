using System;
using LinqToVso.Linqify;
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

        public virtual Dictionary<string, string> GetParameters(LambdaExpression lambdaExpression)
        {
            throw new NotImplementedException("You are calling the base implementation of the request processor. You need to override this method with the specific logic for this request");
        }

        public virtual Request BuildUrl(Dictionary<string, string> expressionParameters)
        {
            throw new NotImplementedException("You are calling the base implementation of the request processor. You need to override this method with the specific logic for this request");
        }

        public virtual List<T> ProcessResults(string apiResponse)
        {
            throw new NotImplementedException("You are calling the base implementation of the request processor. You need to override this method with the specific logic for this request");
        }
    }
}