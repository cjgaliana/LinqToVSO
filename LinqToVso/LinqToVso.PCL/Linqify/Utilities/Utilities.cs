﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LinqToVso.Linqify
{
    public static class Utilities
    {
        /// <summary>
        ///     Assembles a series of key=value pairs as a URI-escaped query-string.
        /// </summary>
        /// <param name="parameters">The parameters to include.</param>
        /// <returns>A query-string-like value such as a=b&c=d.  Does not include a leading question mark (?).</returns>
        public static string BuildQueryString(IEnumerable<QueryParameter> parameters)
        {
            if (parameters == null)
            {
                throw new ArgumentNullException("parameters");
            }

            var builder = new StringBuilder();
            foreach (var pair in parameters.Where(p => !string.IsNullOrWhiteSpace(p.Value)))
            {
                builder.Append(Url.PercentEncode(pair.Name));
                builder.Append('=');
                builder.Append(Url.PercentEncode(pair.Value));
                builder.Append('&');
            }

            if (builder.Length > 1)
            {
                builder.Length--; // truncate trailing &
            }

            return builder.ToString();
        }

        /// <summary>
        /// A simple "Path.Combine-like-ish" method for URLs
        /// </summary>
        /// <param name="paths"></param>
        /// <returns></returns>
        public static string CombineUrls(params string[] paths)
        {
            var sanitizedPaths = paths.Select(x => x.Trim('/')).ToArray();

            var finalString = string.Join("/", sanitizedPaths);
            return finalString;
        }
    }
}