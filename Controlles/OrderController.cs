using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WarehouseManagerAPI.Dtos;
using WarehouseManagerAPI.Services;

namespace WarehouseManagerAPI.Controlles
{
    [Route("api/order")]
    //[Authorize] // TODO disabled for easier testing
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly IOrderService _orderService;

        public OrderController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        [HttpGet]
        public async Task<OkObjectResult> GetOrdersList()
        {
            return Ok(await _orderService.GetOrdersList());
        }

        [HttpGet("{orderId}")]
        public async Task<OrderProductsList> GetOrder([FromRoute] string orderId)
        {
            return new OrderProductsList();
        }

        [HttpPut("pick")]
        public async Task PickItem([FromBody] PickDto pickDto)
        {

        }
    }
}
