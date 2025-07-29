using InventoryManagement.Data.Repositories.Interfaces;
using InventoryManagement.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventoryManagement.Data.Repositories
{
    public class InventoryRepository : Repository<InventoryItem>, IInventoryRepository
    {
        public InventoryRepository(AppDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<InventoryItem>> GetInventoryWithProductsAsync()
        {
            return await _context.InventoryItems
                .Include(i => i.Product)
                .ThenInclude(p => p.Category)
                .Include(i => i.Product)
                .ThenInclude(p => p.Supplier)
                .ToListAsync();
        }

        public async Task<InventoryItem> GetInventoryByProductIdAsync(int productId)
        {
            return await _context.InventoryItems
                .Include(i => i.Product)
                .FirstOrDefaultAsync(i => i.ProductId == productId);
        }

        public async Task<IEnumerable<InventoryItem>> GetLowStockItemsAsync()
        {
            return await _context.InventoryItems
                .Include(i => i.Product)
                .Where(i => i.Quantity <= i.MinimumStock)
                .ToListAsync();
        }
    }
}
