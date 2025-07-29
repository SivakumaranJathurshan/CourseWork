using InventoryManagement.Data.Repositories.Interfaces;
using InventoryManagement.Models;
using InventoryManagement.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventoryManagement.Services
{
    public class InventoryService : IInventoryService
    {
        private readonly IInventoryRepository _inventoryRepository;

        public InventoryService(IInventoryRepository inventoryRepository)
        {
            _inventoryRepository = inventoryRepository;
        }

        public async Task<IEnumerable<InventoryItem>> GetAllInventoryAsync()
        {
            return await _inventoryRepository.GetAllAsync();
        }

        public async Task<InventoryItem> GetInventoryByIdAsync(int id)
        {
            return await _inventoryRepository.GetByIdAsync(id);
        }

        public async Task<InventoryItem> CreateInventoryItemAsync(InventoryItem inventoryItem)
        {
            inventoryItem.CreatedDate = DateTime.UtcNow;
            inventoryItem.UpdatedDate = DateTime.UtcNow;
            inventoryItem.LastRestocked = DateTime.UtcNow;
            return await _inventoryRepository.AddAsync(inventoryItem);
        }

        public async Task<InventoryItem> UpdateInventoryItemAsync(int id, InventoryItem inventoryItem)
        {
            var existingItem = await _inventoryRepository.GetByIdAsync(id);
            if (existingItem == null) return null;

            existingItem.Quantity = inventoryItem.Quantity;
            existingItem.MinimumStock = inventoryItem.MinimumStock;
            existingItem.MaximumStock = inventoryItem.MaximumStock;
            existingItem.UpdatedDate = DateTime.UtcNow;

            return await _inventoryRepository.UpdateAsync(existingItem);
        }

        public async Task<bool> DeleteInventoryItemAsync(int id)
        {
            return await _inventoryRepository.DeleteAsync(id);
        }

        public async Task<IEnumerable<InventoryItem>> GetInventoryWithProductsAsync()
        {
            return await _inventoryRepository.GetInventoryWithProductsAsync();
        }

        public async Task<InventoryItem> GetInventoryByProductIdAsync(int productId)
        {
            return await _inventoryRepository.GetInventoryByProductIdAsync(productId);
        }

        public async Task<IEnumerable<InventoryItem>> GetLowStockItemsAsync()
        {
            return await _inventoryRepository.GetLowStockItemsAsync();
        }

        public async Task<bool> UpdateStockAsync(int productId, int quantity)
        {
            var inventoryItem = await _inventoryRepository.GetInventoryByProductIdAsync(productId);
            if (inventoryItem == null) return false;

            inventoryItem.Quantity += quantity;
            inventoryItem.LastRestocked = DateTime.UtcNow;
            inventoryItem.UpdatedDate = DateTime.UtcNow;

            await _inventoryRepository.UpdateAsync(inventoryItem);
            return true;
        }
    }
}
