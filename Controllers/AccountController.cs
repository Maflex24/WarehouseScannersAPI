using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WarehouseScannersAPI.Dtos;
using WarehouseScannersAPI.Services;

namespace WarehouseScannersAPI.Controllers
{
    [Route("api/account")]
    [Authorize]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly IAccountService _accountService;

        public AccountController(IAccountService accountService)
        {
            _accountService = accountService;
        }


        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<ActionResult<string>> Login([FromBody] EmployeeLoginDto loginDto)
        {
            var token = await _accountService.GenerateJwtToken(loginDto);

            return Ok(token);
        }

        [HttpPost("password")]
        public async Task<ActionResult> ChangePassword([FromBody] EmployeeChangePasswordDto changePasswordDto)
        {
            await _accountService.ChangePassword(changePasswordDto);

            return Ok();
        }
    }
}
