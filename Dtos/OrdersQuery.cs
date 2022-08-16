using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WarehouseScannersAPI.Dtos
{
    public class OrdersQuery
    {
        public int ResultPerQuery { get; set; }
        public int Page { get; set; }
        public DateTime? Created { get; set; }
    }
}
