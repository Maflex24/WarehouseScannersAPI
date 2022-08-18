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
