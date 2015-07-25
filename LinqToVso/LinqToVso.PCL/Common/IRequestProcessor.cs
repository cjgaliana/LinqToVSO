﻿using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace LinqToVso
{
    public interface IRequestProcessor<T>
    {
        string BaseUrl { get; set; }
        string ExtraParameters { get; set; }
        Dictionary<string, string> GetParameters(LambdaExpression lambdaExpression);
        Request BuildUrl(Dictionary<string, string> expressionParameters);
        List<T> ProcessResults(string vsoResponse);
    }

    // temporary marker interface used to communicate that this
    // request processor wants native JSON objects.
    public interface IRequestProcessorWantsJson
    {
    }

    // Declare that this request processor knows how to handle action
    // responses, implies the request processor also wants native JSON objects.
    public interface IRequestProcessorWithAction<T>
        : IRequestProcessorWantsJson
    {
        T ProcessActionResult(string vsoResponse, Enum theAction);
    }
}