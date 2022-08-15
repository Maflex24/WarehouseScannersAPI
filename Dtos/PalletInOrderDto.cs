using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WarehouseScannersAPI.Dtos
{
    public class PalletInOrderDto
    {
        public string PalletId { get; set; }
        public float Weight { get; set; }
    }
}
