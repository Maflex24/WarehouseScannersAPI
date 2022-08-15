using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WarehouseManagerAPI.Dtos
{
    public class NewPalletDto
    {
        public string Id { get; set; }
        public string OrderId { get; set; }
        public int Width { get; set; }
        public int Depth { get; set; }
    }
}
