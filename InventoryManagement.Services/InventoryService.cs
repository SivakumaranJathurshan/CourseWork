using InventoryManagement.Data.Repositories.Interfaces;
using InventoryManagement.Models;
using InventoryManagement.Services.Interfaces;
using InventoryManagement.Services.Utility;
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
        private readonly ILoggerService<InventoryService> _logger;

        public InventoryService(IInventoryRepository inventoryRepository, ILoggerService<InventoryService> logger)
        {
            _inventoryRepository = inventoryRepository;
            _logger = logger;
        }

        public async Task<IEnumerable<InventoryItem>> GetAllInventoryAsync()
        {
            try { 
                return await _inventoryRepository.GetAllAsync();
            }
            catch (Exception ex)
            {
                _logger.LogException("Internal server Error", ex);
                throw new Exception("Internal server Error", ex);
            }
        }

        public async Task<InventoryItem> GetInventoryByIdAsync(int id)
        {
            try { 
                return await _inventoryRepository.GetByIdAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogException("Internal server Error", ex);
                throw new Exception("Internal server Error", ex);
            }
        }

        public async Task<InventoryItem> CreateInventoryItemAsync(InventoryItem inventoryItem)
        {
            try { 
                inventoryItem.CreatedDate = DateTime.UtcNow;
                inventoryItem.UpdatedDate = DateTime.UtcNow;
                inventoryItem.LastRestocked = DateTime.UtcNow;
                return await _inventoryRepository.AddAsync(inventoryItem);
            }
            catch (Exception ex)
            {
                _logger.LogException("Internal server Error", ex);
                throw new Exception("Internal server Error", ex);
            }
        }

        public async Task<InventoryItem> UpdateInventoryItemAsync(int id, InventoryItem inventoryItem)
        {
            try { 
                var existingItem = await _inventoryRepository.GetByIdAsync(id);
                if (existingItem == null) return null;

                existingItem.Quantity = inventoryItem.Quantity;
                existingItem.MinimumStock = inventoryItem.MinimumStock;
                existingItem.MaximumStock = inventoryItem.MaximumStock;
                existingItem.UpdatedDate = DateTime.UtcNow;

                return await _inventoryRepository.UpdateAsync(existingItem);
            }
            catch (Exception ex)
            {
                _logger.LogException("Internal server Error", ex);
                throw new Exception("Internal server Error", ex);
            }
        }

        public async Task<bool> DeleteInventoryItemAsync(int id)
        {
            try { 
                return await _inventoryRepository.DeleteAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogException("Internal server Error", ex);
                throw new Exception("Internal server Error", ex);
            }
        }

        public async Task<IEnumerable<InventoryItem>> GetInventoryWithProductsAsync()
        {
            try { 
                return await _inventoryRepository.GetInventoryWithProductsAsync();
            }
            catch (Exception ex)
            {
                _logger.LogException("Internal server Error", ex);
                throw new Exception("Internal server Error", ex);
            }
        }

        public async Task<InventoryItem> GetInventoryByProductIdAsync(int productId)
        {
            try { 
                return await _inventoryRepository.GetInventoryByProductIdAsync(productId);
            }
            catch (Exception ex)
            {
                _logger.LogException("Internal server Error", ex);
                throw new Exception("Internal server Error", ex);
            }
        }

        public async Task<IEnumerable<InventoryItem>> GetLowStockItemsAsync()
        {
            try { 
                return await _inventoryRepository.GetLowStockItemsAsync();
            }
            catch (Exception ex)
            {
                _logger.LogException("Internal server Error", ex);
                throw new Exception("Internal server Error", ex);
            }
        }

        public async Task<bool> UpdateStockAsync(int productId, int quantity)
        {
            try { 
                var inventoryItem = await _inventoryRepository.GetInventoryByProductIdAsync(productId);
                if (inventoryItem == null) return false;

                inventoryItem.Quantity += quantity;
                inventoryItem.LastRestocked = DateTime.UtcNow;
                inventoryItem.UpdatedDate = DateTime.UtcNow;

                await _inventoryRepository.UpdateAsync(inventoryItem);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogException("Internal server Error", ex);
                throw new Exception("Internal server Error", ex);
            }
        }
    }
}
