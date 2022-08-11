using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WarehouseManagerAPI.Dtos;
using WarehouseManagerAPI.Services;

namespace WarehouseManagerAPI.Entities
{
    [Route("api/storage")]
    //[Authorize] // TODO Temporary turned off to easy testing
    [ApiController]
    public class StorageController : ControllerBase
    {
        private readonly IStorageService _storageService;

        public StorageController(IStorageService storageService)
        {
            _storageService = storageService;
        }

        [HttpGet("empty")]
        public async Task<ActionResult<string>> GetEmptyStorage([FromQuery ]string palletId)
        {
            return await _storageService.GetEmptyStorage(palletId);
        }

        [HttpPost("pallet")]
        public async Task<ActionResult> AssignPalletToStorage([FromQuery] string palletId, [FromQuery] string storageId)
        {
            await _storageService.AssignPalletToStorage(palletId, storageId);
            return Ok();
        }

        [HttpGet("product")]
        public async Task<ActionResult<LocationAndQtyDto>> GetProductLocation([FromQuery] string productId, [FromQuery] int Qty)
        {
            return await _storageService.GetProductLocation(productId, Qty);
        }
    }
}
