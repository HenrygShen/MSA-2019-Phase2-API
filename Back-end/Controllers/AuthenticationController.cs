using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Back_end.Model;
using Back_end.Helper;
using Back_end.DAL;
using Dapper;
using System.Data.SqlClient;
using System.Data;
using System.Net.Http;
using System.Net;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Back_end.Controllers
{
    public class UserDTO
    {
        public int UserId { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
    }
    [Route("api/[controller]")]
    public class AuthenticationController : Controller
    {
        private readonly string connectionString;

        public AuthenticationController()
        {
            this.connectionString = "Server=tcp:msaphase2-hgs.database.windows.net,1433;Initial Catalog=scriber;Persist Security Info=False;User ID=admin-hgs;Password=scriber-7890;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";
        }

        // GET: api/<controller>
        [HttpGet]
        public IEnumerable<Users> Get()
        {
            IEnumerable<Users> users = null;

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                users = connection.Query<Users>("Select * from Users");
            }

            return users;
        }

        // GET api/<controller>/5
        //[HttpGet("{id}")]
        //public async Task<ActionResult<Users>> GetUser(int id)
        //{
            //var user = await _context.Users.FindAsync(id);

            //if (user == null)
            //{
            //    return NotFound();
            //}

            //return user;
        //}

        // POST api/<controller>
        [HttpPost]
        public Task<HttpResponseMessage> Login([FromBody]UserDTO credentials)
        {
            string username = credentials.Username;
            string password = credentials.Password;

            var response = new HttpResponseMessage(HttpStatusCode.OK);

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    string storedPassword = connection.QueryFirstOrDefault<string>("Select Password from Users where Username=@username", new { username });
                    // Check for valid login
                    if (storedPassword == password)
                    {
                        return Task.FromResult(response);
                    }
                    else
                    {
                        response = new HttpResponseMessage(HttpStatusCode.Unauthorized);
                        return Task.FromResult(response);
                    }
                }
            }
            catch
            {
                response = new HttpResponseMessage(HttpStatusCode.BadRequest);
                return Task.FromResult(response);
            }
        }

        // PUT api/<controller>/5
        //[HttpPost("SignUp")]
        //public async Task<ActionResult<Users>> SignUp([FromBody]UserDTO credentials)
        //{
        //    Users user = new Users
        //    {
        //        Username = credentials.Username,
        //        Password = credentials.Password,
        //    };

        //    _context.Users.Add(user);
        //    await _context.SaveChangesAsync();

        //    // Return success code and the info on the user object
        //    return CreatedAtAction("GetUser", new { id = user.UserId }, user);
        //}

        [HttpPost("SignUp")]
        public Task<HttpResponseMessage> SignUp([FromBody]UserDTO credentials)
        {
            string username = credentials.Username;
            string password = credentials.Password;

            var response = new HttpResponseMessage(HttpStatusCode.OK);

            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                response = new HttpResponseMessage(HttpStatusCode.BadRequest);
                return Task.FromResult(response);
            }

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Query<Users>("insert into Users (username, password) values(@username, @password)", new { username, password });
                }
            }
            catch (SqlException ex)
            {
                response = new HttpResponseMessage(HttpStatusCode.BadRequest);
                return Task.FromResult(response);
            }

            return Task.FromResult(response);
        }

        // DELETE api/<controller>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
