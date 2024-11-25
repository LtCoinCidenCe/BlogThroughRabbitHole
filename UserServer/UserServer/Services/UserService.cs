using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;
using UserServer.DBContext;
using UserServer.Models;

namespace UserServer.Services
{
    public class UserService(UserContext userContext)
    {
        public IEnumerable<User> GetAll()
        {
            return userContext.Users.AsNoTracking().Select(
                u => new User() { ID = u.ID, Username = u.Username, Passhash = string.Empty }
                );
        }

        public User? Get(long id)
        {
            return userContext.Users.FirstOrDefault(u => u.ID == id);
        }

        public User? Get(string username)
        {
            return userContext.Users.FirstOrDefault(u => u.Username == username);
        }

        public User Create(User newUser)
        {
            newUser.Passhash = getHash(newUser.Passhash);
            userContext.Users.Add(newUser);
            userContext.SaveChanges();
            return newUser;
        }

        public void UpdatePassword(long id, string password)
        {
            userContext.Users.Where(u => u.ID == id).
                ExecuteUpdate(setters => setters.SetProperty(
                    u => u.Passhash, getHash(password)
                    )
                );
        }

        public void Remove(long id)
        {
            userContext.Users.Where(u => u.ID == id).ExecuteDelete();
        }

        private string getHash(string password)
        {
            var hashed = SHA256.HashData(Encoding.UTF8.GetBytes(password));
            hashed = SHA256.HashData(hashed);
            StringBuilder builder = new StringBuilder();
            for (int i = 0; i < hashed.Length; i++)
            {
                builder.Append($"{hashed[i]:X2}");
            }
            return builder.ToString();
        }
    }
}
