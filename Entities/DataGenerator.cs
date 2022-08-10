using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using WarehouseManagerAPI.Dtos;
using WarehouseManagerAPI.Services;

namespace WarehouseManagerAPI.Entities
{
    public class DataGenerator
    {
        private readonly WarehouseManagerDbContext _dbContext;
        private readonly IAccountService _accountService;

        public DataGenerator(WarehouseManagerDbContext dbContext, IAccountService accountService)
        {
            _dbContext = dbContext;
            _accountService = accountService;
        }

        public async Task GeneratePermissions()
        {
            var permissions = new List<Permission>()
            {
                new Permission() {Name = "picking"},
                new Permission() {Name = "inbound"},
                new Permission() {Name = "outbound"},
            };

            var existedPermissionsCount = _dbContext.Permissions.Count();
            if (existedPermissionsCount == permissions.Count)
                return;

            var existedPermissions = await _dbContext.Permissions.Select(p => p.Name).ToListAsync();
            foreach (var newPermission in permissions.Where(newPermissionsType =>
                         !existedPermissions.Contains(newPermissionsType.Name)))
            {
                _dbContext.Permissions.Add(newPermission);
            }

            await _dbContext.SaveChangesAsync();
        }

        public async Task AddProducts()
        {
            var products = new List<Product>()
            {
                new Product()
                {
                    Id = "15ActiveSpeaker",
                    Name = "15' Active 2-way speaker",
                    Weight = 14.6f,
                    Width = 450,
                    Depth = 370,
                    Height = 690,
                },
                new Product()
                {
                    Id = "10ActiveSpeaker",
                    Name = "10' Active 2-way speaker",
                    Weight = 15.0f,
                    Width = 361,
                    Depth = 310,
                    Height = 495,
                },
                new Product()
                {
                    Id = "8ActiveSpeaker",
                    Name = "8' Active 2-way speaker",
                    Weight = 7.9f,
                    Width = 313,
                    Depth = 258,
                    Height = 486,
                },
                new Product()
                {
                    Id = "18ActiveSub",
                    Name = "Active 18' Subwoofer",
                    Weight = 45f,
                    Width = 530,
                    Depth = 580,
                    Height = 693,
                },
                new Product()
                {
                    Id = "2x18ActiveSub",
                    Name = "2x Active 18' Subwoofer",
                    Weight = 92.5f,
                    Width = 550,
                    Depth = 680,
                    Height = 1200,
                }
            };

            var existedProductsCount = _dbContext.Products.Count();
            if (existedProductsCount == products.Count)
                return;

            await _dbContext.Products.AddRangeAsync(products);
            await _dbContext.SaveChangesAsync();
        }

        public async Task AddOrders()
        {
            var orders = new List<Order>();
            var random = new Random(1024);

            for (int i = 0; i < 50; i++)
            {
                var newOrder = new Order
                {
                    Created = DateTime.Now.AddMinutes(-random.Next(1, 4320)),
                    Status = "Released"
                };

                newOrder.Id = newOrder.Created.ToString("yyMMddhhmmss");
                orders.Add(newOrder);
            }

            var existedOrdersCount = _dbContext.Orders.Count();
            if (existedOrdersCount == orders.Count)
                return;

            await _dbContext.Orders.AddRangeAsync(orders);
            await _dbContext.SaveChangesAsync();
        }

        public async Task GenerateOrderPositions()
        {
            var products = await _dbContext
                .Products
                .Select(p => p.Id)
                .ToListAsync();

            var orders = await _dbContext
                .Orders
                .Select(o => o.Id)
                .ToListAsync();

            List<OrderPosition> orderPositions = new List<OrderPosition>();
            var productsTypesQty = products.Count();
            var random = new Random(1024);

            foreach (var orderId in orders)
            {
                var howManyProducts = random.Next(1, productsTypesQty);

                for (var i = 0; i < howManyProducts; i++)
                {
                    var randomTrue = random.Next(100);
                    if (randomTrue > 50)
                        continue;

                    orderPositions.Add(new OrderPosition()
                    {
                        OrderId = orderId,
                        ProductId = products[i],
                        Qty = random.Next(1, 50),
                        Completed = false
                    });
                }
            }

            await _dbContext.OrderPositions.AddRangeAsync(orderPositions);
            await _dbContext.SaveChangesAsync();
        }

        public async Task CreateStorages()
        {
            var highStorageNames = new List<string>()
            {
                "A1-01-A01",
                "A1-01-B01",
                "A1-01-C01",
                "A1-01-A02",
                "A1-01-B02",
                "A1-01-C02",
                "A1-01-A03",
                "A1-01-B03",
                "A1-01-C03",
            };

            var lowStorageNames = new List<string>()
            {
                "A1-02-A01",
                "A1-02-B01",
                "A1-02-C01",
                "A1-02-A02",
                "A1-02-B02",
                "A1-02-C02",
                "A1-02-A03",
                "A1-02-B03",
                "A1-02-C03",
            };

            var wideStorageNames = new List<string>()
            {
                "A1-03-A01",
                "A1-03-B01",
                "A1-03-C01",
                "A1-03-A02",
                "A1-03-B02",
                "A1-03-C02",
                "A1-03-A03",
                "A1-03-B03",
                "A1-03-C03",
            };

            var storages = new List<Storage>();

            foreach (var storageName in highStorageNames)
            {
                storages.Add(new Storage()
                {
                    Id = storageName,
                    MaxWeight = 1500,
                    Depth = 1200,
                    Height = 2300,
                    Width = 850
                });   
            }

            foreach (var storageName in lowStorageNames)
            {
                storages.Add(new Storage()
                {
                    Id = storageName,
                    MaxWeight = 700,
                    Depth = 1200,
                    Height = 1600,
                    Width = 850
                });
            }

            foreach (var storageName in wideStorageNames)
            {
                storages.Add(new Storage()
                {
                    Id = storageName,
                    MaxWeight = 1500,
                    Depth = 1200,
                    Height = 2300,
                    Width = 1100
                });
            }

            await _dbContext.Storages.AddRangeAsync(storages);
            await _dbContext.SaveChangesAsync();
        }

        public async Task AddStorageContent()
        {
            var storages = await _dbContext
                .Storages
                .ToListAsync();

            var products = await _dbContext
                .Products
                .ToListAsync();

            var random = new Random(1024);
            var storageContents = new List<StorageContent>();

            foreach (var storage in storages)
            {
                var randomTrue = random.Next(100);
                if (randomTrue > 60)
                    continue;

                var product = products[random.Next(products.Count())];

                var storageVolume = (long)storage.Width * (long)storage.Height * (long)storage.Depth;
                var productVolume = product.Width * product.Height * product.Depth;
                var productQty = storageVolume / (long)productVolume;

                storageContents.Add(new StorageContent()
                {
                    Product = product,
                    Storage = storage,
                    Qty = (int)productQty
                });
            }

            await _dbContext.StorageContents.AddRangeAsync(storageContents);
            await _dbContext.SaveChangesAsync();
        }
    }
}
