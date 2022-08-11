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
    public interface IStorageService
    {
        public Task<string> GetEmptyStorage(string palletId);
        public Task AssignPalletToStorage(string palletId, string storageId);
        public Task<LocationAndQtyDto> GetProductLocation(string productId, int Qty);
    }

    public class StorageService : IStorageService
    {
        private readonly WarehouseManagerDbContext _dbContext;

        public StorageService(WarehouseManagerDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<string> GetEmptyStorage(string palletId)
        {
            var pallet = await _dbContext
                .Pallets
                .SingleOrDefaultAsync(p => p.Id == palletId);

            if (pallet == null)
                throw new BadRequestException("Pallet doesn't exist");

            var storage = await _dbContext
                .Storages
                .Include(s => s.StorageContent)
                .Where(s => !s.StorageContent.Any() && !s.Temporary)
                .OrderBy(s => s.Height)
                .Where(s => 
                    s.MaxWeight > pallet.Weight &&
                    s.Depth >= pallet.Depth &&
                    s.Height >= pallet.Height &&
                    s.Width >= pallet.Width)
                .FirstOrDefaultAsync();

            return storage == null ? null : storage.Id;
        }

        public async Task AssignPalletToStorage(string palletId, string storageId)
        {
            var storage = await _dbContext
                .Storages
                .Include(s => s.StorageContent)
                .SingleOrDefaultAsync(s => s.Id == storageId);

            if (storage == null)
                throw new BadRequestException("Storage doesn't exist");

            if (storage.StorageContent.Any())
                throw new BadRequestException("Storage isn't empty");

            var pallet = await _dbContext
                .Pallets
                .Include(p => p.PalletContent)
                .SingleOrDefaultAsync(p => p.Id == palletId);

            if (pallet == null)
                throw new BadRequestException("Pallet doesn't exist");

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
        }

        public async Task<LocationAndQtyDto> GetProductLocation(string productId, int Qty)
        {
            var storageContent = await _dbContext
                .StorageContents
                .Where(sc =>
                    sc.ProductId == productId &&
                    sc.Qty >= Qty)
                .OrderBy(sc => sc.Qty)
                .FirstOrDefaultAsync();

            if (storageContent != null)
                return new LocationAndQtyDto()
                {
                    StorageId = storageContent.StorageId,
                    Qty = Qty
                };

            storageContent = await _dbContext
                .StorageContents
                .Where(sc => sc.ProductId == productId)
                .OrderByDescending(sc => sc.Qty)
                .FirstOrDefaultAsync();

            if (storageContent == null)
                throw new Exception("There is no more product on storage");

            return new LocationAndQtyDto()
            {
                StorageId = storageContent.StorageId,
                Qty = storageContent.Qty
            };
        }
    }
}
