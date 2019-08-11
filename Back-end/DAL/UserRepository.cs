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
        bool Register(string username, string password);
    }

    public class UserRepository : IUserRepository
    {
        private readonly string connectionString;

        public UserRepository()
        {
            this.connectionString = "Server=tcp:msaphase2-hgs.database.windows.net,1433;Initial Catalog=scriber;Persist Security Info=False;User ID=admin-hgs;Password=scriber-7890;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";
        }

        public async Task<User> Authenticate(string username, string password)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    var user = connection.QuerySingleOrDefaultAsync<User>("Select * from Users where Username=@username", new { username }).Result;
                    // Check for valid login, returns null if user not found
                    if (user == null)
                        return null;

                    // authentication successful so return user details without password
                    user.Password = null;
                    return user;
                }
            }
            catch
            {
                return null;
            }
            //var user = await Task.Run(() => _users.SingleOrDefault(x => x.Username == username && x.Password == password));
        }

        public bool Register(string username, string password)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    var affectedRows = connection.ExecuteAsync(@"insert into Users (username, password) 
                                                        values(@username, @password)", new { username, password }).Result;
                    Console.WriteLine(affectedRows);
                    return true;
                }
            }
            catch (SqlException ex)
            {
                return false;
            }
        }
    }
}
