using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WarehouseScannersAPI.Dtos
{
    public class PickDto
    {
        public string ProductId { get; set; }
        public int Qty { get; set; }
        public string StorageId { get; set; }
        public string PalletId { get; set; }
    }
}
