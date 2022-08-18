using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WarehouseScannersAPI.Dtos;
using WarehouseScannersAPI.Services;


namespace WarehouseScannersAPI.Controllers
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
        public async Task<OkObjectResult> GetOrdersList([FromQuery] OrdersQuery ordersQuery)
        {
            return Ok(await _orderService.GetOrdersList(ordersQuery));
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
