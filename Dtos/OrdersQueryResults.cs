namespace WarehouseScannersAPI.Dtos
{
    public class OrdersQueryResults
    {
        public int TotalOrders { get; set; }
        public int TotalPages { get; set; }
        public int OrdersFrom { get; set; }
        public int OrdersTo { get; set; }
        public List<OrdersListPositionDto> Orders { get; set; }

        public OrdersQueryResults(OrdersQuery ordersQuery, List<OrdersListPositionDto> ordersList, int totalOrders)
        {
            Orders = ordersList;
            TotalOrders = totalOrders;
            TotalPages = (int)Math.Ceiling(this.TotalOrders / (double)ordersQuery.ResultPerQuery);
            OrdersFrom = ordersQuery.ResultPerQuery * (ordersQuery.Page - 1) + 1;
            OrdersTo = this.OrdersFrom + ordersQuery.ResultPerQuery - 1;
        }
    }
}
