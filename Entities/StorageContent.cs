namespace WarehouseScannersAPI.Entities
{
    public class StorageContent
    {
        public int Id { get; set; }
        public string StorageId { get; set; }
        public virtual Storage Storage { get; set; }

        public string ProductId { get; set; }
        public virtual Product Product { get; set; }

        public int Qty { get; set; }
    }
}
