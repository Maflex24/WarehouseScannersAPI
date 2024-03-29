﻿namespace WarehouseScannersAPI.Entities
{
    public class PalletContent
    {
        public int Id { get; set; }
        public int Qty { get; set; }

        public string PalletId { get; set; }
        public virtual Pallet Pallet { get; set; }

        public string ProductId { get; set; }
        public virtual Product Product { get; set; }
    }
}
