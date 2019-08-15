using Back_end.Model;
using Dapper;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace Back_end.DAL
{
    public interface IUserRepository
    {
        Task<User> Authenticate(string username, string password);
        Task<bool> Register(string username, string password);
    }

    public class UserRepository : IUserRepository
    {
        private readonly string connectionString;

        public UserRepository()
        {
            this.connectionString = "Server=tcp:scriber-hgs.database.windows.net,1433;Initial Catalog=scriber;Persist Security Info=False;User ID=admin-hgs;Password=scriber-7890;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";
        }

        public async Task<User> Authenticate(string username, string password)
        {
            
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    await connection.OpenAsync();
                    var user = await connection.QuerySingleOrDefaultAsync<User>("Select * from Users where Username=@username", new { username });
                    // Check for valid login, returns null if user not found
                    if (user == null)
                        return null;

                    bool isValid = BCrypt.Net.BCrypt.Verify(password, user.Password);
                    if (isValid)
                    {
                        user.Password = null;
                        return user;
                    }
                    else
                    {
                        return null;
                    }
                }
            }
            catch
            {
                return null;
            }
        }

        public async Task<bool> Register(string username, string password)
        {
            string hashedPW = BCrypt.Net.BCrypt.HashPassword(password, workFactor: 12);
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    await connection.OpenAsync();
                    string response = await connection.QuerySingleOrDefaultAsync<string>("Select 1 from Users where Username=@username", new { username });
                    if (response == "1")
                    {
                        return false;
                    }
                    await connection.ExecuteAsync(@"insert into Users (username, password) values(@username, @hashedPW)", new { username, hashedPW });
                    return true;
                }
            }
            catch
            {
                return false;
            }
        }
    }
}
