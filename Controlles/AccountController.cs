using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WarehouseManagerAPI.Dtos;
using WarehouseManagerAPI.Entities;
using WarehouseManagerAPI.Services;

namespace WarehouseManagerAPI.Controlles
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
