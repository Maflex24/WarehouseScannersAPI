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
    [Route("api/order")]
    [Authorize] 
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly IOrderService _orderService;

        public OrderController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        [HttpGet]
        [Authorize(Policy = "Picker")]
        public async Task<OkObjectResult> GetOrdersList()
        {
            return Ok(await _orderService.GetOrdersList());
        }

        [HttpGet("{orderId}")]
        [Authorize(Policy = "Picker")]
        public async Task<OkObjectResult> GetOrder([FromRoute] string orderId)
        {
            return Ok(await _orderService.GetOrder(orderId));
        }

        [HttpPut("pick")]
        [Authorize(Policy = "Picker")]
        public async Task PickItem([FromBody] PickDto pickDto)
        {
            await _orderService.PickItem(pickDto);
        }

        [HttpPost("pallet")]
        [Authorize(Policy = "Picker")]
        public async Task<ActionResult> AddPallet(NewPalletDto newPallet)
        {
            var pallet = await _orderService.AddPallet(newPallet);
            return Created(pallet.Id, pallet);
        }
    }
}
