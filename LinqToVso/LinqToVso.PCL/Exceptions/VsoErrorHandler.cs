using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using LinqToVso.Exceptions;

namespace LinqToVso
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
            var responseStr = await httpResponseMessage.Content.ReadAsStringAsync().ConfigureAwait(false);

            var error = ParseVsoErrorMessage(responseStr);

            var message = error.Message +
                          " - Please visit the LINQ to VSO FAQ (at the HelpLink) and open a issue for help on resolving this error.";

            throw new LinqToVsoQueryException(message)
            {
                HelpLink = "https://github.com/cjgaliana/LinqToVSO",
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

            return new VsoErrorDetails {Message = response};
        }
    }
}