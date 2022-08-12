using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using WarehouseManagerAPI.Dtos;
using WarehouseManagerAPI.Entities;
using WarehouseManagerAPI.Exceptions;

namespace WarehouseManagerAPI.Services
{
    public interface IOrderService
    {
        public Task<List<OrdersListPositionDto>> GetOrdersList();
        public Task<OrderProductsList> GetOrder(string orderId);
        public Task PickItem(PickDto pickDto);
    }

    public class OrderService : IOrderService
    {
        private readonly WarehouseManagerDbContext _dbContext;

        public OrderService(WarehouseManagerDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<List<OrdersListPositionDto>> GetOrdersList()
        {
            var orders = await _dbContext
                .Orders
                .Include(o => o.OrderPositions)
                .Where(o => o.Status == "Released" && o.OrderPositions.Any())
                .OrderBy(o => o.Created)
                .ToListAsync();

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

            return ordersList;
        }

        public async Task<OrderProductsList> GetOrder(string orderId)
        {
            var orderPositions = await _dbContext
                .OrderPositions
                .Where(op => op.OrderId == orderId && !op.Completed)
                .Include(op => op.Product)
                .Select(op => new OrderPositionDto()
                {
                    ProductId = op.ProductId,
                    Qty = op.Qty,
                    Weight = op.Product.Weight
                })
                .OrderByDescending(op => op.Weight)
                .ToListAsync();

            if (orderPositions == null)
                return null;

            return new OrderProductsList()
            {
                OrderId = orderId,
                OrderPositions = orderPositions
            };
        }

        public async Task PickItem(PickDto pickDto)
        {
            var storage = await _dbContext
                .Storages
                .Include(s => s.StorageContent)
                .SingleOrDefaultAsync(s => s.Id == pickDto.StorageId);

            var pallet = await _dbContext
                .Pallets
                .Include(p => p.PalletContent)
                .SingleOrDefaultAsync(p => p.Id == pickDto.PalletId);

            var product = await _dbContext
                .Products
                .SingleOrDefaultAsync(p => p.Id == pickDto.ProductId);

            if (product == null || pallet == null || storage == null)
                throw new BadRequestException("Storage/pallet/product don't exist");

            var storageQty = storage.StorageContent.FindAll(sc => sc.ProductId == pickDto.ProductId).Select(pq => pq.Qty).Sum();

            if (storageQty < pickDto.Qty)
                throw new BadRequestException("There is not enough product Qty on this storage");

            pallet.PalletContent.Add(new PalletContent()
            {
                ProductId = pickDto.ProductId,
                Qty = pickDto.Qty
            });

            var storageContent =
                storage.StorageContent.FirstOrDefault(sc => sc.ProductId == pickDto.ProductId && sc.Qty >= pickDto.Qty);

            storageContent.Qty -= pickDto.Qty;

            pallet.Weight += product.Weight * pickDto.Qty;

            await _dbContext.SaveChangesAsync();
        }
    }
}
