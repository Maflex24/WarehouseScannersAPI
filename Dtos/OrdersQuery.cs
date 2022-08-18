namespace WarehouseScannersAPI.Dtos
{
    public class OrdersQuery
    {
        public int ResultPerQuery { get; set; }
        public int Page { get; set; }
        public DateTime? Created { get; set; }
    }
}
