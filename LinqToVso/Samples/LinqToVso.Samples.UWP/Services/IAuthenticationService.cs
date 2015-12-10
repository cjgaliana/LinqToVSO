using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Windows.Security.Authentication.Web;

namespace LinqToVso.Samples.UWP.Services
{
    public interface IAuthenticationService
    {
        Task<string> LoginAsync();

        Task LoginAsync(string account, string username, string password);
    }

    public class BasicAuthenticationService : IAuthenticationService
    {
        public Task<string> LoginAsync()
        {
            throw new NotImplementedException("User Basic authentication");
        }

        public async Task LoginAsync(string account, string username, string password)
        {
            CheckValidParameters(account, username, password);

            await EnsureValidCredentials(account, username, password);
        }

        /// <summary>
        ///     Random call to the API to check if the credentials are valid
        /// </summary>
        /// <param name="account"></param>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        private async Task EnsureValidCredentials(string account, string username, string password)
        {
            using (var client = new HttpClient())
            {
                client.Timeout = TimeSpan.FromSeconds(5);
                client.DefaultRequestHeaders.Accept.Add(
                    new MediaTypeWithQualityHeaderValue("application/json"));

                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic",
                    Convert.ToBase64String(
                        Encoding.ASCII.GetBytes($"{username}:{password}")));

                // Get 1 project from default collection
                var url =
                    $"https://{account}.visualstudio.com/defaultcollection/_apis/projects?api-version={1.0}&$top={1}";

                var response = await client.GetAsync(url);
                response.EnsureSuccessStatusCode();
                var responseBody = await response.Content.ReadAsStringAsync();
            }
        }

        private void CheckValidParameters(string account, string username, string password)
        {
            if (string.IsNullOrWhiteSpace(account))
            {
                throw new ArgumentNullException(account);
            }
            if (string.IsNullOrWhiteSpace(username))
            {
                throw new ArgumentNullException(username);
            }
            if (string.IsNullOrWhiteSpace(password))
            {
                throw new ArgumentNullException(password);
            }
        }
    }

    /// <summary>
    ///     Not supported in mobile apps. For more info, see https://www.visualstudio.com/integrate/get-started/auth/oauth
    /// </summary>
    [Obsolete("Use Basic authentication instead")]
    public class WebAuthenticationBrokerService : IAuthenticationService
    {
        private string appID = "";
        private string appSecret = "";
        private readonly Uri authorizeUri = new Uri("https://app.vssps.visualstudio.com/oauth2/authorize");
        private Uri tokenURI = new Uri("https://app.vssps.visualstudio.com/oauth2/token");

        public async Task<string> LoginAsync()
        {
            string result;

            try
            {
                var webAuthenticationResult =
                    await WebAuthenticationBroker.AuthenticateAsync(
                        WebAuthenticationOptions.None,
                        authorizeUri);

                switch (webAuthenticationResult.ResponseStatus)
                {
                    case WebAuthenticationStatus.Success:
                        // Successful authentication.
                        result = webAuthenticationResult.ResponseData;
                        break;

                    case WebAuthenticationStatus.ErrorHttp:
                        // HTTP error.
                        result = webAuthenticationResult.ResponseErrorDetail.ToString();
                        break;

                    default:
                        // Other error.
                        result = webAuthenticationResult.ResponseData;
                        break;
                }
            }
            catch (Exception ex)
            {
                // Authentication failed. Handle parameter, SSL/TLS, and Network Unavailable errors here.
                result = ex.Message;
            }

            return result;
        }

        public Task LoginAsync(string account, string username, string password)
        {
            throw new NotImplementedException("Use Oaut2 authentication");
        }
    }
}