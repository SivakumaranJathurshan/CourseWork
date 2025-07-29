using InventoryManagement.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventoryManagement.Data.Repositories.Interfaces
{
    public interface IInventoryRepository : IRepository<InventoryItem>
    {
        Task<IEnumerable<InventoryItem>> GetInventoryWithProductsAsync();
        Task<InventoryItem> GetInventoryByProductIdAsync(int productId);
        Task<IEnumerable<InventoryItem>> GetLowStockItemsAsync();
    }
}
