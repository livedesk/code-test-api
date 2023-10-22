using code_test_api.Dtos;
using code_test_api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace code_test_api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpPost]
        [Route("register")]
        public ActionResult Register(UserForRegisterDto model)
        {
            var userExists = _userService.GetUser(model.Username) != null;

            if (userExists)
                return BadRequest("User already exists");

            var createdUser = _userService.Register(model);

            return Ok(createdUser);
        }

        [HttpPost]
        [Route("login")]
        public ActionResult Login(UserForLoginDto model)
        {
            // Get JWT string from login or null if not success
            string? jwtToken = _userService.Login(model);

            if (jwtToken == null)
                return Unauthorized();
            
            return Ok(jwtToken);
        }

        [HttpGet]
        [Authorize]
        public ActionResult FetchUserData()
        {
            // Get the username from the claim in the passed in JWT Bearer token
            var userName = User.Claims.FirstOrDefault(c => c.Type.Contains("nameidentifier"))?.Value;

            // Guard username not found
            if (userName == null)
                return BadRequest();

            // Get the user by username
            var user = _userService.GetUser(userName);

            // Guard user not found
            if (user == null)
                return BadRequest();

            return Ok(user);
        }
    }
}
