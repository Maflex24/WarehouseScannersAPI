using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using WarehouseManagerAPI.Entities;

namespace WarehouseManagerAPI.Services
{
    public interface IStorageService
    {
        public Task<string> GetEmptyStorage(string palletId);
        public Task AssignPalletToStorage(string palletId, string storageId);
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
                throw new Exception("Pallet doesn't exist");

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
                throw new Exception("Storage doesn't exist");

            if (storage.StorageContent.Any())
                throw new Exception("Storage isn't empty");

            var pallet = await _dbContext
                .Pallets
                .Include(p => p.PalletContent)
                .SingleOrDefaultAsync(p => p.Id == palletId);

            if (pallet == null)
                throw new Exception("Pallet doesn't exist");

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
    }
}
