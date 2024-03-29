﻿using Microsoft.EntityFrameworkCore;
using WarehouseScannersAPI.Dtos;
using WarehouseScannersAPI.Entities;
using WarehouseScannersAPI.Exceptions;

namespace WarehouseScannersAPI.Services
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
        private readonly WarehouseScannersDbContext _dbContext;
        private readonly ILogger<OrderService> _logger;

        public OrderService(WarehouseScannersDbContext dbContext, ILogger<OrderService> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        public async Task<OrdersQueryResults> GetOrdersList(OrdersQuery ordersQuery)
        {
            var query = _dbContext
                .Orders
                .AsNoTracking()
                .Where(o => o.Status == "Released" && o.OrderPositions.Any());

            if (ordersQuery.Created != null)
                query = query
                    .Where(o => o.Created.Date == ordersQuery.Created);

            var orders = await query
                .OrderBy(o => o.Created)
                .Skip(ordersQuery.Page * ordersQuery.ResultPerQuery - ordersQuery.ResultPerQuery)
                .Take(ordersQuery.ResultPerQuery)
                .Include(o => o.OrderPositions)
                .ToListAsync();

            int totalOrders;
            if (ordersQuery.Created != null)
                totalOrders = _dbContext.Orders.Where(o => o.Created.Date == ordersQuery.Created)
                    .Count(o => o.Status == "Released" && o.OrderPositions.Any());
            else
                totalOrders = _dbContext.Orders.Count(o => o.Status == "Released" && o.OrderPositions.Any());

            var products = await _dbContext
                .Products
                .AsNoTracking()
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
                .AsNoTracking()
                .Where(op => op.OrderId == orderId && !op.Completed)
                .Include(op => op.Product)
                .ToListAsync();

            var palletsInOrder = await _dbContext
                .Pallets
                .AsNoTracking()
                .Where(p => p.OrderId == orderId)
                .Select(p => new PalletInOrderDto()
                {
                    PalletId = p.Id,
                    Weight = p.Weight
                })
                .ToListAsync();

            float productsWeight = 0, productsVolume = 0;

            foreach (var orderPosition in orderPositions)
            {
                productsWeight += orderPosition.Product.Weight * (orderPosition.Qty - orderPosition.PickedQty);
                productsVolume += orderPosition.Product.Volume * (orderPosition.Qty - orderPosition.PickedQty);
            }

            var orderPositionsDto = orderPositions
                .Select(op => new OrderPositionDto()
                {
                    ProductId = op.ProductId,
                    PickedQty = op.PickedQty,
                    ToPick = op.Qty - op.PickedQty,
                    SingleWeight = op.Product.Weight
                })
                .OrderByDescending(op => op.SingleWeight)
                .ToList();

            return new OrderProductsList()
            {
                OrderId = orderId,
                ProductsWeightLeft = productsWeight,
                ProductsVolumeLeft = productsVolume,
                OrderPositions = orderPositionsDto,
                PickedPallets = palletsInOrder
            };
        }

        public async Task PickItem(PickDto pickDto)
        {
            var storage = await GetStorageWithContent(pickDto.StorageId);
            var pallet = await GetPalletWithContent(pickDto.PalletId);
            var product = await GetProduct(pickDto.ProductId);

            var storageProductAvailableQty = storage.StorageContent
                .Where(sc => sc.ProductId == product.Id)
                .Select(sc => sc.Qty)
                .FirstOrDefault();

            if (storageProductAvailableQty < pickDto.Qty)
                throw new BadRequestException($"There is not enough [{pickDto.ProductId}] qty on this storage. It's [{storageProductAvailableQty}], you want to pick [{pickDto.Qty}]");

            var orderPosition = await _dbContext
                .OrderPositions
                .Include(op => op.Order)
                .SingleOrDefaultAsync(op => op.OrderId == pallet.OrderId && op.ProductId == pickDto.ProductId);

            if (orderPosition is null)
                throw new BadRequestException($"Product {product.Id} is not ordered in current order!");

            if (orderPosition.Qty - orderPosition.PickedQty < pickDto.Qty)
                throw new BadRequestException($"You try to pick more items than is in the order. To pick left: [{orderPosition.Qty - orderPosition.PickedQty}]. Tried to pick: [{pickDto.Qty}]");

            pallet.PalletContent.Add(new PalletContent()
            {
                ProductId = pickDto.ProductId,
                Qty = pickDto.Qty
            });

            orderPosition.PickedQty += pickDto.Qty;
            if (orderPosition.PickedQty == orderPosition.Qty)
                orderPosition.Completed = true;

            var order = orderPosition.Order;

            if (order.OrderPositions.All(op => op.Completed))
                order.Status = "Completed";

            var storageContent =
                storage.StorageContent.FirstOrDefault(sc => sc.ProductId == pickDto.ProductId && sc.Qty >= pickDto.Qty);

            storageContent.Qty -= pickDto.Qty;

            if (storageContent.Qty == 0)
                _dbContext.Remove(storageContent);

            pallet.Weight += product.Weight * pickDto.Qty;

            await _dbContext.SaveChangesAsync();

            _logger.LogInformation($"PICKING | Picked [{pickDto.Qty}] of [{pickDto.ProductId}] from [{pickDto.StorageId}] to pallet [{pickDto.PalletId}] and order [{pallet.OrderId}]");
        }

        private async Task<Storage> GetStorageWithContent(string storageId)
        {
            var storage = await _dbContext
                .Storages
                .Include(s => s.StorageContent)
                .SingleOrDefaultAsync(s => s.Id == storageId);

            if (storage == null) throw new BadRequestException($"Storage {storageId} does not exist");

            return storage;
        }

        private async Task<Pallet> GetPalletWithContent(string palletId)
        {
            var pallet = await _dbContext
                .Pallets
                .Include(p => p.PalletContent)
                .SingleOrDefaultAsync(p => p.Id == palletId);

            if (pallet == null) throw new BadRequestException($"Pallet {palletId} does not exist");

            return pallet;
        }

        private async Task<Product> GetProduct(string productId)
        {
            var product = await _dbContext
                .Products
                .SingleOrDefaultAsync(p => p.Id == productId);

            if (product == null) throw new BadRequestException($"Product {productId} does not exist");

            return product;
        }

        public async Task<Pallet> AddPallet(NewPalletDto newPallet)
        {
            var order = await _dbContext
                .Orders
                .SingleOrDefaultAsync(o => o.Id == newPallet.OrderId);

            if (order == null)
                throw new BadRequestException($"Order [{newPallet.OrderId}] doesn't exist");

            if(newPallet.Depth < 300 || newPallet.Width < 600)
                throw new BadRequestException("Not valid pallet dimensions");

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

            _logger.LogInformation($"PALLET | Added pallet {pallet.Id} to order {pallet.OrderId}");
            return pallet;
        }
    }
}
