using Microsoft.EntityFrameworkCore;
using WarehouseScannersAPI.Dtos;
using WarehouseScannersAPI.Entities;
using WarehouseScannersAPI.Exceptions;

namespace WarehouseScannersAPI.Services
{
    public interface IStorageService
    {
        public Task<string> GetEmptyStorage(string palletId);
        public Task AssignPalletToStorage(string palletId, string storageId);
        public Task<LocationAndQtyDto> GetProductLocation(string productId, int qty);
    }

    public class StorageService : IStorageService
    {
        private readonly WarehouseScannersDbContext _dbContext;
        private readonly ILogger<StorageService> _logger;

        public StorageService(WarehouseScannersDbContext dbContext, ILogger<StorageService> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        public async Task<string> GetEmptyStorage(string palletId)
        {
            var pallet = await _dbContext
                .Pallets
                .SingleOrDefaultAsync(p => p.Id == palletId);

            if (pallet == null)
                throw new BadRequestException($"Pallet [{palletId}] doesn't exist");

            var storage = await _dbContext
                .Storages
                .Where(s => !s.StorageContent.Any() && !s.Temporary)
                .OrderBy(s => s.Height)
                .Where(s => 
                    s.MaxWeight > pallet.Weight &&
                    s.Depth >= pallet.Depth &&
                    s.Height >= pallet.Height &&
                    s.Width >= pallet.Width)
                .FirstOrDefaultAsync();

            if (storage == null)
                throw new Exception("There is no more valid empty storage space");

            return storage.Id;
        }

        public async Task AssignPalletToStorage(string palletId, string storageId)
        {
            var storage = await _dbContext
                .Storages
                .Include(s => s.StorageContent)
                .SingleOrDefaultAsync(s => s.Id == storageId);

            if (storage == null)
                throw new BadRequestException($"Storage [{storageId}] doesn't exist");

            if (storage.StorageContent.Any())
                throw new BadRequestException($"Storage [{storageId}] isn't empty");

            var pallet = await _dbContext
                .Pallets
                .Include(p => p.PalletContent)
                .SingleOrDefaultAsync(p => p.Id == palletId);

            if (pallet == null)
                throw new BadRequestException($"Pallet [{palletId}] doesn't exist");

            foreach (var palletContent in pallet.PalletContent)
            {
                storage.StorageContent.Add(new StorageContent()
                {
                    ProductId = palletContent.ProductId,
                    Qty = palletContent.Qty,
                });
            }

            _dbContext.Pallets.Remove(pallet);
            _dbContext.PalletContents.RemoveRange(pallet.PalletContent);
            await _dbContext.SaveChangesAsync();

            _logger.LogInformation($"INBOUND | Pallet [{palletId}] assigned to [{storageId}]");
        }

        public async Task<LocationAndQtyDto> GetProductLocation(string productId, int qty)
        {
            var storageContent = await _dbContext
                .StorageContents
                .Where(sc =>
                    sc.ProductId == productId &&
                    sc.Qty >= qty)
                .OrderBy(sc => sc.Qty)
                .FirstOrDefaultAsync();

            if (storageContent != null)
                return new LocationAndQtyDto()
                {
                    StorageId = storageContent.StorageId,
                    Qty = qty
                };

            storageContent = await _dbContext
                .StorageContents
                .Where(sc => sc.ProductId == productId)
                .OrderByDescending(sc => sc.Qty)
                .FirstOrDefaultAsync();

            if (storageContent == null)
                throw new Exception($"There is no more product [{productId}] on storage");

            return new LocationAndQtyDto()
            {
                StorageId = storageContent.StorageId,
                Qty = storageContent.Qty
            };
        }
    }
}
