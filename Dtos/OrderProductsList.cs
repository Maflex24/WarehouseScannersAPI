using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WarehouseManagerAPI.Dtos
{
    public class OrderProductsList
    {
        public string OrderId { get; set; }
        public List<OrderPositionDto> OrderPositions { get; set; }
    }
}
