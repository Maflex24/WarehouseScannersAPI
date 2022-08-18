namespace WarehouseScannersAPI.Dtos
{
    public class NewPalletDto
    {
        public string Id { get; set; }
        public string OrderId { get; set; }
        public int Width { get; set; }
        public int Depth { get; set; }
    }
}
