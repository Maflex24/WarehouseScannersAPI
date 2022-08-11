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
    }
}
