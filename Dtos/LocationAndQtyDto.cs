using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WarehouseScannersAPI.Dtos
{
    public class LocationAndQtyDto
    {
        public string StorageId { get; set; }
        public int Qty { get; set; }
    }
}
