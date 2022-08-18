using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WarehouseScannersAPI.Dtos;

namespace WarehouseScannersAPI.Dtos
{
    public class OrderProductsList
    {
        public string OrderId { get; set; }
        public float ProductsWeightLeft { get; set; }
        public float ProductsVolumeLeft { get; set; }
        public List<OrderPositionDto> OrderPositions { get; set; }
        public List<PalletInOrderDto> PickedPallets { get; set; }

    }
}
