using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WarehouseScannersAPI.Dtos;
using WarehouseScannersAPI.Services;

namespace WarehouseScannersAPI.Entities
{
    [Route("api/storage")]
    [Authorize] 
    [ApiController]
    public class StorageController : ControllerBase
    {
        private readonly IStorageService _storageService;

        public StorageController(IStorageService storageService)
        {
            _storageService = storageService;
        }

        [HttpGet("empty")]
        [Authorize(Policy = "Inbound")]
        public async Task<ActionResult<string>> GetEmptyStorage([FromQuery ]string palletId)
        {
            return Ok(await _storageService.GetEmptyStorage(palletId));
        }

        [HttpPost("pallet")]
        [Authorize(Policy = "Inbound")]
        public async Task<ActionResult> AssignPalletToStorage([FromQuery] string palletId, [FromQuery] string storageId)
        {
            await _storageService.AssignPalletToStorage(palletId, storageId);
            return Ok();
        }

        [HttpGet("product")]
        [Authorize(Policy = "Picker")]
        public async Task<ActionResult<LocationAndQtyDto>> GetProductLocation([FromQuery] string productId, [FromQuery] int Qty)
        {
            return Ok(await _storageService.GetProductLocation(productId, Qty));
        }
    }
}
