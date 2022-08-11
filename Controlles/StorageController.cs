using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WarehouseManagerAPI.Entities
{
    [Route("api/storage")]
    [Authorize]
    [ApiController]
    public class StorageController : ControllerBase
    {
        [HttpGet("empty")]
        public async Task<ActionResult<string>> GetEmptyStorage([FromQuery ]string palletId)
        {
            return "A01";
        }

        [HttpPost("pallet")]
        public async Task<ActionResult> AssignPalletToStorage([FromQuery] string palletId, [FromQuery] string storageId)
        {
            return Ok();
        }

        [HttpGet("product")]
        public async Task<ActionResult<string>> GetProductLocation([FromQuery] string productId, [FromQuery] int Qty)
        {
            return "A01...";
        }
    }
}
