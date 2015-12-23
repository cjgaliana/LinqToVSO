using System.Collections.Generic;
using System.Linq.Expressions;

namespace LinqToVso.Linqify
{
    public interface IRequestProcessor<T>
    {
        string BaseUrl { get; set; }

        string ApiVersion { get; set; }

        IList<CustomApiParameter> CustomParameters { get; set; }

        Dictionary<string, string> GetParameters(LambdaExpression lambdaExpression);

        Request BuildUrl(Dictionary<string, string> expressionParameters);

        List<T> ProcessResults(string apiResponse);
    }
}