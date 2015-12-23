using LinqToVso.Linqify;
using System.Collections.Generic;

namespace LinqToVso.Extensions
{
    public static class QueryParameterExtensions
    {
        /// <summary>
        /// Adds the special "API_VERSION" query parameter to the parameter list of the request
        /// </summary>
        /// <param name="request"></param>
        /// <param name="apiVersion"></param>
        public static void AddApiVersionParameter(this Request request, string apiVersion)
        {
            request.AddParameter(VsoQueryConstans.VSO_API_VERSION_TAG, apiVersion);
        }
    }
}