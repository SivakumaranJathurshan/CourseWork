using InventoryManagement.Models;
using InventoryManagement.Models.DTO;

namespace InventoryManagement.Services.Interfaces
{
    public interface IInventoryService
    {
        Task<IEnumerable<InventoryItem>> GetAllInventoryAsync();
        Task<InventoryItem> GetInventoryByIdAsync(int id);
        Task<InventoryItem> CreateInventoryItemAsync(InventoryItemCreateDTO inventoryItem);
        Task<InventoryItem> UpdateInventoryItemAsync(int id, InventoryItemUpdateDTO inventoryItem);
        Task<bool> DeleteInventoryItemAsync(int id);
        Task<IEnumerable<InventoryItem>> GetInventoryWithProductsAsync();
        Task<InventoryItem> GetInventoryByProductIdAsync(int productId);
        Task<IEnumerable<InventoryItem>> GetLowStockItemsAsync();
        Task<bool> UpdateStockAsync(int productId, int quantity);
    }
}
