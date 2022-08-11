using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WarehouseManagerAPI.Dtos;

namespace WarehouseManagerAPI.Controlles
{
    [Route("api/order")]
    [Authorize]
    [ApiController]
    public class OrderController : ControllerBase
    {
        [HttpGet]
        public async Task<List<OrdersListPositionDto>> GetOrdersList()
        {
            return new List<OrdersListPositionDto>();
        }

        [HttpGet("{orderId}")]
        public async Task<OrderProductsList> GetOrder([FromRoute] string orderId)
        {
            return new OrderProductsList();
        }
    }
}
