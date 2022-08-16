using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using WarehouseManagerAPI.Dtos;
using WarehouseManagerAPI.Entities;
using WarehouseManagerAPI.Exceptions;
using WarehouseScannersAPI.Dtos;

namespace WarehouseManagerAPI.Services
{
    public interface IOrderService
    {
        public Task<OrdersQueryResults> GetOrdersList(OrdersQuery ordersQuery);
        public Task<OrderProductsList> GetOrder(string orderId);
        public Task PickItem(PickDto pickDto);
        public Task<Pallet> AddPallet(NewPalletDto newPallet);
    }

    public class OrderService : IOrderService
    {
        private readonly WarehouseManagerDbContext _dbContext;

        public OrderService(WarehouseManagerDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<OrdersQueryResults> GetOrdersList(OrdersQuery ordersQuery)
        {
            var orders = await _dbContext
                .Orders
                .Where(o => o.Status == "Released" && o.OrderPositions.Any())
                .OrderBy(o => o.Created)
                .Skip(ordersQuery.Page * ordersQuery.ResultPerQuery - ordersQuery.ResultPerQuery)
                .Take(ordersQuery.ResultPerQuery)
                .Include(o => o.OrderPositions)
                .ToListAsync();

            var totalOrders = _dbContext
                .Orders
                .Count(o => o.Status == "Released" && o.OrderPositions.Any());

            var products = await _dbContext
                .Products
                .Select(p => new {Id = p.Id, Volume = p.Volume, Weight = p.Weight})
                .ToListAsync();

            var ordersList = new List<OrdersListPositionDto>();

            foreach (var order in orders)
            {
                var orderListPosition = new OrdersListPositionDto()
                {
                    OrderId = order.Id,
                    Created = order.Created,
                    ProductsQty = order.OrderPositions.Count(),
                };

                float weight = 0, volume = 0;
                var qty = 0;

                foreach (var orderPosition in order.OrderPositions)
                {
                    var product = products.Single(p => p.Id == orderPosition.ProductId);
                    qty += orderPosition.Qty;
                    weight += orderPosition.Qty * product.Weight;
                    volume += orderPosition.Qty * product.Volume;
                }

                orderListPosition.ProductsWeight = weight;
                orderListPosition.ProductsVolume = volume;
                orderListPosition.ProductsQty = qty;

                ordersList.Add(orderListPosition);
            }

            return new OrdersQueryResults(ordersQuery, ordersList, totalOrders);
        }

        public async Task<OrderProductsList> GetOrder(string orderId)
        {
            var orderExist = _dbContext
                .Orders
                .Any(o => o.Id == orderId);

            if (!orderExist)
                throw new BadRequestException($"Order id {orderId} doesn't exist");

            var orderPositions = await _dbContext
                .OrderPositions
                .Where(op => op.OrderId == orderId && !op.Completed)
                .Include(op => op.Product)
                .Select(op => new OrderPositionDto()
                {
                    ProductId = op.ProductId,
                    PickedQty = op.PickedQty,
                    ToPick = op.Qty - op.PickedQty,
                    SingleWeight = op.Product.Weight
                })
                .OrderByDescending(op => op.SingleWeight)
                .ToListAsync();

            var palletsInOrder = await _dbContext
                .Pallets
                .Where(p => p.OrderId == orderId)
                .Select(p => new PalletInOrderDto()
                {
                    PalletId = p.Id,
                    Weight = p.Weight
                })
                .ToListAsync();

            float productsWeight = 0, productsVolume = 0;

            var products = await _dbContext
                .Products
                .Select(p => new {Id = p.Id, Volume = p.Volume})
                .ToListAsync();

            foreach (var orderPosition in orderPositions)
            {
                productsWeight += orderPosition.SingleWeight * orderPosition.ToPick;
                productsVolume += products.FirstOrDefault(p => p.Id == orderPosition.ProductId).Volume * orderPosition.ToPick;
            }

            return new OrderProductsList()
            {
                OrderId = orderId,
                ProductsWeightLeft = productsWeight,
                ProductsVolumeLeft = productsVolume,
                OrderPositions = orderPositions,
                PickedPallets = palletsInOrder
            };
        }

        public async Task PickItem(PickDto pickDto)
        {
            var storage = await _dbContext
                .Storages
                .Include(s => s.StorageContent)
                .SingleOrDefaultAsync(s => s.Id == pickDto.StorageId);

            if (storage == null) throw new BadRequestException($"Storage {pickDto.ProductId} does not exist");

            var pallet = await _dbContext
                .Pallets
                .Include(p => p.PalletContent)
                .SingleOrDefaultAsync(p => p.Id == pickDto.PalletId);

            if (pallet == null) throw new BadRequestException($"Pallet {pickDto.PalletId} does not exist");

            var product = await _dbContext
                .Products
                .SingleOrDefaultAsync(p => p.Id == pickDto.ProductId);

            if (product == null) throw new BadRequestException($"Product {pickDto.ProductId} does not exist");

            var storageProductAvailableQty = storage.StorageContent
                .Where(sc => sc.ProductId == product.Id)
                .Select(sc => sc.Qty)
                .FirstOrDefault();

            if (storageProductAvailableQty < pickDto.Qty)
                throw new BadRequestException("There is not enough product qty on this storage");

            var orderPosition = await _dbContext
                .OrderPositions
                .FirstOrDefaultAsync(op => op.OrderId == pallet.OrderId && op.ProductId == pickDto.ProductId);

            if (orderPosition is null)
                throw new BadRequestException($"Product {product.Id} is not ordered in current order!");

            if (orderPosition.Qty - orderPosition.PickedQty < pickDto.Qty)
                throw new BadRequestException("You try to pick more items than is in the order");

            pallet.PalletContent.Add(new PalletContent()
            {
                ProductId = pickDto.ProductId,
                Qty = pickDto.Qty
            });

            orderPosition.PickedQty += pickDto.Qty;
            if (orderPosition.PickedQty == orderPosition.Qty)
                orderPosition.Completed = true;

            var order = await _dbContext
                .Orders
                .Include(o => o.OrderPositions)
                .SingleOrDefaultAsync(o => o.Id == pallet.OrderId);

            if (order.OrderPositions.All(op => op.Completed))
                order.Status = "Completed";

            var storageContent =
                storage.StorageContent.FirstOrDefault(sc => sc.ProductId == pickDto.ProductId && sc.Qty >= pickDto.Qty);

            storageContent.Qty -= pickDto.Qty;

            if (storageContent.Qty == 0)
                _dbContext.Remove(storageContent);

            pallet.Weight += product.Weight * pickDto.Qty;

            await _dbContext.SaveChangesAsync();
        }

        public async Task<Pallet> AddPallet(NewPalletDto newPallet)
        {
            var order = await _dbContext
                .Orders
                .SingleOrDefaultAsync(o => o.Id == newPallet.OrderId);

            if (order == null)
                throw new BadRequestException("Order id doesn't exist");

            var pallet = new Pallet()
            {
                Id = newPallet.Id,
                OrderId = newPallet.OrderId,
                Depth = newPallet.Depth,
                Width = newPallet.Width,
                Height = 0
            };

            await _dbContext.Pallets.AddAsync(pallet);
            await _dbContext.SaveChangesAsync();

            return pallet;
        }
    }
}
