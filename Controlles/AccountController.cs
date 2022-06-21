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
    [Route("api/employee")]
    [Authorize]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly IAccountService _accountService;

        public AccountController(IAccountService accountService)
        {
            _accountService = accountService;
        }

        [HttpPost("register")]
        public async Task<ActionResult<Employee>> NewEmployee([FromBody] EmployeeRegisterPost employee)
        {
            var newEmployee =  await _accountService.AddEmployee(employee);

            if (newEmployee == null)
                return BadRequest();

            return Created($"api/employee/{newEmployee.Id}", newEmployee);
        }

        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<ActionResult<string>> Login([FromBody] EmployeeLoginDto loginDto)
        {
            var token = await _accountService.GenerateJwtToken(loginDto);

            return Ok(token);
        }

        
    }
}
