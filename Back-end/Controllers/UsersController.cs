using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Threading.Tasks;
using Back_end.DAL;
using Back_end.Model;

namespace WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private IUserRepository _userRepository;

        public UsersController(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        [HttpPost("Authenticate")]
        public async Task<IActionResult> Authenticate([FromBody]UserDTO userParam)
        {
            var user = await _userRepository.Authenticate(userParam.Username, userParam.Password);

            if (user == null)
                return BadRequest(new { message = "Username or password is incorrect" });

            return Ok(user);
        }

        [HttpPost("Signup")]
        public async Task<IActionResult> SignUp([FromBody]UserDTO userParam)
        {
            var registered = _userRepository.Register(userParam.Username, userParam.Password);

            if (registered == false)
                return BadRequest(new { message = "Registration failed, Username taken" });

            return Ok(new { message = "Registration Successful" });
        }
    }
}