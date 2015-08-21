using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace LinqToVso.PCL.Exceptions
{
    public class VsoErrorHandler
    {
        public static async Task ThrowIfErrorAsync(HttpResponseMessage httpResponseMessage)
        {
            //Handle more possible errors in the response
            switch (httpResponseMessage.StatusCode)
            {
                case HttpStatusCode.Unauthorized:
                    await HandleUnauthorizedAsync(httpResponseMessage).ConfigureAwait(false);
                    break;
            }
        }

        private static async Task HandleUnauthorizedAsync(HttpResponseMessage httpResponseMessage)
        {
            string responseStr = await httpResponseMessage.Content.ReadAsStringAsync().ConfigureAwait(false);

            VsoErrorDetails error = ParseVsoErrorMessage(responseStr);

            string message = error.Message + " - Please visit the LINQ to Twitter FAQ (at the HelpLink) for help on resolving this error.";

            throw new LinqToVsoQueryException(message)
            {
                HelpLink = "https://linqtotwitter.codeplex.com/wikipage?title=LINQ%20to%20Twitter%20FAQ",
                ErrorCode = error.Code,
                StatusCode = HttpStatusCode.Unauthorized,
                ReasonPhrase = httpResponseMessage.ReasonPhrase
            };
        }

        private static VsoErrorDetails ParseVsoErrorMessage(string response)
        {
            //if (response.StartsWith("{"))
            //{
            //    JsonData responseJson = JsonMapper.ToObject(response);

            //    var errors = responseJson.GetValue<JsonData>("errors");

            //    if (errors != null)
            //    {
            //        if (errors.GetJsonType() == JsonType.String)
            //            return new VsoErrorDetails
            //            {
            //                Message = responseJson.GetValue<string>("errors"),
            //                Code = -1
            //            };

            //        if (errors.Count > 0)
            //        {
            //            var error = errors[0];
            //            return new VsoErrorDetails
            //            {
            //                Message = error.GetValue<string>("message"),
            //                Code = error.GetValue<int>("code")
            //            };
            //        }
            //    }
            //}

            return new VsoErrorDetails { Message = response };
        }

      
    }
}