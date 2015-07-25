using System.Collections.Generic;
using System.Linq.Expressions;

namespace LinqToVso
{
    public interface IRequestProcessor<T>
    {
        string BaseUrl { get; set; }

        IList<string> IncludeParameters { get; set; }

        Dictionary<string, string> GetParameters(LambdaExpression lambdaExpression);

        Request BuildUrl(Dictionary<string, string> expressionParameters);

        List<T> ProcessResults(string vsoResponse);
    }
}