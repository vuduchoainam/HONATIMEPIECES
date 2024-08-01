using HONATIMEPIECES.Helpers;
using Microsoft.AspNetCore.Mvc;
using SharedClassLibrary.Contracts;
using SharedClassLibrary.DTOs;

namespace HONATIMEPIECES.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class AccountController(IUserAccount userAccount) : ControllerBase
    {
        [HttpPost("Register")]
        public async Task<IActionResult> Register(UserDTO userDTO)
        {
            var response = await userAccount.CreateAccount(userDTO);
            return StatusCodeResponse.SuccessResponse(response);
        }

        [HttpPost("Login")]
        public async Task<IActionResult> Login(LoginDTO loginDTO)
        {
            var response = await userAccount.LoginAccount(loginDTO);
            return StatusCodeResponse.SuccessResponse(response);
        }
    }
}
