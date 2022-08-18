using Microsoft.EntityFrameworkCore;
using WarehouseScannersAPI.Services;

namespace WarehouseScannersAPI.Entities
{
    public class DataGenerator
    {
        private readonly WarehouseScannersDbContext _dbContext;
        private readonly IAccountService _accountService;
        private readonly Random random;

        public DataGenerator(WarehouseScannersDbContext dbContext, IAccountService accountService)
        {
            _dbContext = dbContext;
            _accountService = accountService;
            random = new Random(1024);
        }

        public async Task Seeder()
        {
            var ordersAmount = 100;
            var existedOrdersAmount = _dbContext.Orders.Count();

            if (ordersAmount <= existedOrdersAmount)
                return;

            await GeneratePermissions();
            await AddAcounts();
            await AddProducts();
            await AddOrders(ordersAmount);
            await CreateStorages();
            await AddStorageContent();
            await CreatePalletsWithContent(20, "Euro8010", 2200, 800);
            await CreatePalletsWithContent(20, "EuroLow8010", 650, 800);
            await CreatePalletsWithContent(10, "Block1010", 2200, 1000);
        }

        public async Task GeneratePermissions()
        {
            var permissions = new List<Permission>()
            {
                new Permission() {Name = "picking"},
                new Permission() {Name = "inbound"},
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

        public async Task AddAcounts()
        {
            await _accountService.AddEmployee("inbound", "123456", "Inbound operator");
            await _accountService.AddEmployee("picker", "123456", "Picker");
            await _accountService.AddEmployee("double", "123456", "Double");

            var inboundAccount = await _dbContext.Accounts.Include(a => a.Permissions).SingleOrDefaultAsync(a => a.Login == "inbound");
            var pickerAccount = await _dbContext.Accounts.Include(a => a.Permissions).SingleOrDefaultAsync(a => a.Login == "picker");
            var doubleAccount = await _dbContext.Accounts.Include(a => a.Permissions).SingleOrDefaultAsync(a => a.Login == "double");
            var permissions = await _dbContext.Permissions.ToListAsync();

            inboundAccount.Permissions.Add(permissions.SingleOrDefault(p => p.Name == "inbound"));
            pickerAccount.Permissions.Add(permissions.SingleOrDefault(p => p.Name == "picking"));
            doubleAccount.Permissions.Add(permissions.SingleOrDefault(p => p.Name == "picking"));
            doubleAccount.Permissions.Add(permissions.SingleOrDefault(p => p.Name == "inbound"));

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
                    Id = "15PassiveSpeaker",
                    Name = "15' Passive 2-way speaker",
                    Weight = 11.4f,
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
                    Id = "10PassiveSpeaker",
                    Name = "10' Passive 2-way speaker",
                    Weight = 12.0f,
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
                    Id = "8PassiveSpeaker",
                    Name = "8' Active 2-way speaker",
                    Weight = 5.9f,
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
                    Id = "18PassiveSub",
                    Name = "Passive 18' Subwoofer",
                    Weight = 38f,
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
                },
                new Product()
                {
                    Id = "2x18PassiveSub",
                    Name = "2x Passive 18' Subwoofer",
                    Weight = 84.5f,
                    Width = 550,
                    Depth = 680,
                    Height = 1200,
                }
            };

            var existedProductsCount = _dbContext.Products.Count();
            if (existedProductsCount == products.Count)
                return;

            foreach (var product in products)
            {
                var height = (double) product.Height / 100;
                var width = (double) product.Width / 100;
                var depth = (double) product.Depth / 100;

                product.Volume = (float)Math.Round(height * width * depth, 2);
            }

            await _dbContext.Products.AddRangeAsync(products);
            await _dbContext.SaveChangesAsync();
        }

        public async Task AddOrders(int ordersAmount)
        {
            var existedOrdersCount = _dbContext.Orders.Count();
            if (existedOrdersCount == ordersAmount)
                return;

            var orders = new List<Order>();

            for (var i = 0; i < ordersAmount; i++)
            {
                var newOrder = new Order
                {
                    Created = DateTime.Now.AddSeconds(-random.Next(1, 345600)),
                    Status = "Released"
                };

                newOrder.Id = newOrder.Created.ToString("yyMMdd-hhmmss-" + i.ToString().PadLeft(2, '0'));
                orders.Add(newOrder);

            }

            await _dbContext.Orders.AddRangeAsync(orders);
            await _dbContext.SaveChangesAsync();

            await GenerateOrderPositions();
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

            foreach (var orderId in orders)
            {
                var howManyProducts = random.Next(1, productsTypesQty);

                for (var i = 0; i < howManyProducts; i++)
                {
                    var randomTrue = random.Next(100);
                    if (randomTrue > 65)
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

                "A1-02-A01",
                "A1-02-B01",
                "A1-02-C01",
                "A1-02-A02",
                "A1-02-B02",
                "A1-02-C02",
                "A1-02-A03",
                "A1-02-B03",
                "A1-02-C03",

                "A1-03-A01",
                "A1-03-B01",
                "A1-03-C01",
                "A1-03-A02",
                "A1-03-B02",
                "A1-03-C02",
                "A1-03-A03",
                "A1-03-B03",
                "A1-03-C03",

                "A1-04-A01",
                "A1-04-B01",
                "A1-04-C01",
                "A1-04-A02",
                "A1-04-B02",
                "A1-04-C02",
                "A1-04-A03",
                "A1-04-B03",
                "A1-04-C03",

                "A1-05-A01",
                "A1-05-B01",
                "A1-05-C01",
                "A1-05-A02",
                "A1-05-B02",
                "A1-05-C02",
                "A1-05-A03",
                "A1-05-B03",
                "A1-05-C03",
            };

            var lowStorageNames = new List<string>()
            {
                "A1-06-A01",
                "A1-06-B01",
                "A1-06-C01",
                "A1-06-D01",
                "A1-06-A02",
                "A1-06-B02",
                "A1-06-C02",
                "A1-06-D02",
                "A1-06-A03",
                "A1-06-B03",
                "A1-06-C03",
                "A1-06-D03",

                "A1-07-A01",
                "A1-07-B01",
                "A1-07-C01",
                "A1-07-D01",
                "A1-07-A02",
                "A1-07-B02",
                "A1-07-C02",
                "A1-07-D02",
                "A1-07-A03",
                "A1-07-B03",
                "A1-07-C03",
                "A1-07-D03",

                "A1-08-A01",
                "A1-08-B01",
                "A1-08-C01",
                "A1-08-D01",
                "A1-08-A02",
                "A1-08-B02",
                "A1-08-C02",
                "A1-08-D02",
                "A1-08-A03",
                "A1-08-B03",
                "A1-08-C03",
                "A1-08-D03",

                "A1-09-A01",
                "A1-09-B01",
                "A1-09-C01",
                "A1-09-D01",
                "A1-09-A02",
                "A1-09-B02",
                "A1-09-C02",
                "A1-09-D02",
                "A1-09-A03",
                "A1-09-B03",
                "A1-09-C03",
                "A1-09-D03",
            };

            var wideStorageNames = new List<string>()
            {
                "A1-10-A01",
                "A1-10-B01",
                "A1-10-C01",
                "A1-10-A02",
                "A1-10-B02",
                "A1-10-C02",
                "A1-10-A03",
                "A1-10-B03",
                "A1-10-C03",

                "A1-11-A01",
                "A1-11-B01",
                "A1-11-C01",
                "A1-11-A02",
                "A1-11-B02",
                "A1-11-C02",
                "A1-11-A03",
                "A1-11-B03",
                "A1-11-C03",

                "A1-12-A01",
                "A1-12-B01",
                "A1-12-C01",
                "A1-12-A02",
                "A1-12-B02",
                "A1-12-C02",
                "A1-12-A03",
                "A1-12-B03",
                "A1-12-C03",

                "A1-13-A01",
                "A1-13-B01",
                "A1-13-C01",
                "A1-13-A02",
                "A1-13-B02",
                "A1-13-C02",
                "A1-13-A03",
                "A1-13-B03",
                "A1-13-C03",
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
                if (randomTrue > 70)
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

        public async Task CreatePalletsWithContent(int howManyPallets, string startedNumber, int height, int width)
        {
            var products = await _dbContext
                .Products
                .ToListAsync();

            var pallets = new List<Pallet>();
            var palletsContent = new List<PalletContent>();
            var random = new Random(1024);

            for (int i = 0; i < howManyPallets; i++)
            {
                var product = products[random.Next(products.Count())];
                var pallet = new Pallet()
                {
                    Id = startedNumber + i,
                    Width = width,
                    Depth = 1200,
                    Height = height
                };

                var palletVolume = (long)pallet.Width * (long)pallet.Height * (long)pallet.Depth;
                var productVolume = product.Width * product.Height * product.Depth;
                var productQty = palletVolume/ (long)productVolume;

                pallet.Weight = productQty * product.Weight + 25;

                pallets.Add(pallet);
                palletsContent.Add(new PalletContent()
                {
                    Pallet = pallet,
                    Product = product,
                    Qty = (int)productQty
                });
            }

            await _dbContext.Pallets.AddRangeAsync(pallets);
            await _dbContext.PalletContents.AddRangeAsync(palletsContent);
            await _dbContext.SaveChangesAsync();
        }
    }
}
