using System.Threading.Tasks;
using LinqToVso.PCL.Authorization;
using System;
using System.Collections.Generic;
using System.Threading;

namespace LinqToVso.PCL.Context
{
    public interface IVsoExecute
    {
        /// <summary>
        /// Gets or sets the object that can send authorized requests to VSO.
        /// </summary>
        IAuthorizer Authorizer { get; }

        /// <summary>
        /// Allows callers to cancel operation (where applicable)
        /// </summary>
        CancellationToken CancellationToken { get; set; }

        /// <summary>
        /// Gets the most recent URL executed
        /// </summary>
        /// <remarks>
        /// This is very useful for debugging
        /// </remarks>
        Uri LastUrl { get; }

        /// <summary>
        /// list of response headers from query
        /// </summary>
        IDictionary<string, string> ResponseHeaders { get; set; }

        /// <summary>
        /// Gets and sets HTTP UserAgent header
        /// </summary>
        string UserAgent { get; set; }

        /// <summary>
        /// Timeout (milliseconds) for writing to request
        /// stream or reading from response stream
        /// </summary>
        int ReadWriteTimeout { get; set; }

        /// <summary>
        /// Timeout (milliseconds) to wait for a server response
        /// </summary>
        int Timeout { get; set; }


        /// <summary>
        /// makes HTTP call to VSO API
        /// </summary>
        /// <param name="url">URL with all query info</param>
        /// <param name="reqProc">Request Processor for Async Results</param>
        /// <returns>JSON Results from VSO</returns>
        Task<string> QueryVsoAsync<T>(Request req, IRequestProcessor<T> reqProc);

        /// <summary>
        /// performs HTTP POST to VSO
        /// </summary>
        /// <param name="url">URL of request</param>
        /// <param name="postData">parameters to post</param>
        /// <param name="getResult">callback for handling async Json response - null if synchronous</param>
        /// <returns>Json Response from VSO - empty string if async</returns>
        Task<string> PostToVsoAsync<T>(string url, IDictionary<string, string> postData, CancellationToken cancelToken);

    }
}