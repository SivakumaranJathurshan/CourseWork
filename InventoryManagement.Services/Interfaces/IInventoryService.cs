using InventoryManagement.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventoryManagement.Services.Interfaces
{
    public interface IInventoryService
    {
        Task<IEnumerable<InventoryItem>> GetAllInventoryAsync();
        Task<InventoryItem> GetInventoryByIdAsync(int id);
        Task<InventoryItem> CreateInventoryItemAsync(InventoryItem inventoryItem);
        Task<InventoryItem> UpdateInventoryItemAsync(int id, InventoryItem inventoryItem);
        Task<bool> DeleteInventoryItemAsync(int id);
        Task<IEnumerable<InventoryItem>> GetInventoryWithProductsAsync();
        Task<InventoryItem> GetInventoryByProductIdAsync(int productId);
        Task<IEnumerable<InventoryItem>> GetLowStockItemsAsync();
        Task<bool> UpdateStockAsync(int productId, int quantity);
    }
}
