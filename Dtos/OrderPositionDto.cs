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
