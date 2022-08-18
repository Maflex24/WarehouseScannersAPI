using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WarehouseScannersAPI.Dtos
{
    public class OrderPositionDto
    {
        public string ProductId { get; set; }
        public int PickedQty { get; set; }
        public int ToPick { get; set; }
        public float SingleWeight { get; set; }
    }
}
