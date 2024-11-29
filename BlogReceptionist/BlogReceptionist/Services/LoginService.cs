using BlogReceptionist.Models;
using BlogReceptionist.Utilities;
using System.Net;

namespace BlogReceptionist.Services
{
    public class LoginService
    {
        public string apiURL = $"https://{Env.USERURL}/api/login/";
        public HttpClient httpClient;
        public LoginService()
        {
            httpClient = new HttpClient()
            {
                BaseAddress = new Uri(apiURL)
            };
        }

        public bool TryLogin(User newUser,out string token)
        {
            token = string.Empty;
            Task<HttpResponseMessage> result = httpClient.PostAsJsonAsync(string.Empty, newUser);
            result.Wait();
            if (!result.IsCompletedSuccessfully)
            {
                return false;
            }
            if (result.Result.StatusCode != HttpStatusCode.OK)
            {
                return false;
            }
            token = result.Result.Content.ReadAsStringAsync().Result;
            return true;
        }
    }
}
