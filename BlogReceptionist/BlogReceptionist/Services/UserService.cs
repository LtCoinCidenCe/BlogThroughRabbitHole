using BlogReceptionist.Models;
using BlogReceptionist.Utilities;
using System.Net;

namespace BlogReceptionist.Services
{
    public class UserService
    {
        public string apiURL = $"http://{Env.USERHOST}/api/user/";
        public HttpClient httpClient;
        public UserService()
        {
            httpClient = new HttpClient()
            {
                BaseAddress = new Uri(apiURL)
            };
        }

        public async Task<User?> GetUser(string username)
        {
            HttpResponseMessage conn = await httpClient.GetAsync(username);
            if (conn.StatusCode != HttpStatusCode.OK)
            {
                return null;
            }
            User? result = await conn.Content.ReadFromJsonAsync<User>();
            return result;
        }

        public async Task<User?> GetUser(long id)
        {
            HttpResponseMessage conn = await httpClient.GetAsync($"id/{id}");
            if (conn.StatusCode != HttpStatusCode.OK)
            {
                return null;
            }
            User? result = await conn.Content.ReadFromJsonAsync<User>();
            return result;
        }
    }
}
