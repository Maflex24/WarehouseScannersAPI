using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WarehouseManagerAPI.Dtos
{
    public class OrdersListPositionDto
    {
        public string OrderId { get; set; }
        public DateTime Created { get; set; }
        public int ProductsQty { get; set; }
        public float ProductsWeight { get; set; }
        public float ProductsVolume { get; set; }
    }
}
