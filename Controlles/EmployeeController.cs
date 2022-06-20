using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services.Services;
using WarehouseManagerAPI.Dtos;
using WarehouseManagerAPI.Entities;

namespace WarehouseManagerAPI.Controlles
{
    [Route("api/employees")]
    [ApiController]
    public class EmployeeController : ControllerBase
    {
        private readonly IEmployeeService _employeeService;

        public EmployeeController(IEmployeeService employeeService)
        {
            _employeeService = employeeService;
        }

        [HttpPost("register")]
        public async Task<ActionResult<Employee>> NewEmployee([FromBody] EmployeeRegisterPost employee)
        {
            var newEmployee =  await _employeeService.AddEmployee(employee);

            if (newEmployee == null)
                return BadRequest();

            return Created($"api/employee/{newEmployee.Id}", newEmployee);
        }

        [HttpPost("login")]
        public async Task<ActionResult<string>> Login([FromBody] EmployeeLoginDto loginDto)
        {
            var token = await _employeeService.GenerateJwtToken(loginDto);

            return Ok(token);
        }

        
    }
}
