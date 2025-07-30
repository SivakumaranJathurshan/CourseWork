using InventoryManagement.Data.Repositories.Interfaces;
using InventoryManagement.Models;
using InventoryManagement.Services.Interfaces;
using InventoryManagement.Services.Utility;

namespace InventoryManagement.Services
{
    /// <summary>
    /// Service class responsible for managing inventory-related business logic.
    /// </summary>
    public class InventoryService : IInventoryService
    {
        private readonly IInventoryRepository _inventoryRepository;
        private readonly ILoggerService<InventoryService> _logger;

        /// <summary>
        /// Constructor for InventoryService.
        /// </summary>
        /// <param name="inventoryRepository">Repository to access inventory data.</param>
        /// <param name="logger">Logger service for exception logging.</param>
        public InventoryService(IInventoryRepository inventoryRepository, ILoggerService<InventoryService> logger)
        {
            _inventoryRepository = inventoryRepository;
            _logger = logger;
        }

        /// <summary>
        /// Retrieves all inventory items.
        /// </summary>
        /// <returns>A list of all inventory items.</returns>
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

        /// <summary>
        /// Retrieves a specific inventory item by its ID.
        /// </summary>
        /// <param name="id">The ID of the inventory item.</param>
        /// <returns>The matching inventory item or null if not found.</returns>
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

        /// <summary>
        /// Creates a new inventory item.
        /// </summary>
        /// <param name="inventoryItem">The inventory item to create.</param>
        /// <returns>The created inventory item.</returns>
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

        /// <summary>
        /// Updates an existing inventory item.
        /// </summary>
        /// <param name="id">The ID of the inventory item to update.</param>
        /// <param name="inventoryItem">The updated inventory data.</param>
        /// <returns>The updated inventory item, or null if not found.</returns>
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

        /// <summary>
        /// Deletes an inventory item by its ID.
        /// </summary>
        /// <param name="id">The ID of the inventory item to delete.</param>
        /// <returns>True if deletion was successful; otherwise, false.</returns>
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

        /// <summary>
        /// Retrieves all inventory items along with their related product data.
        /// </summary>
        /// <returns>A list of inventory items with associated products.</returns>
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

        /// <summary>
        /// Retrieves an inventory item based on the associated product ID.
        /// </summary>
        /// <param name="productId">The ID of the product.</param>
        /// <returns>The matching inventory item, or null if not found.</returns>
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

        /// <summary>
        /// Retrieves all inventory items that are below the minimum stock level.
        /// </summary>
        /// <returns>A list of low stock inventory items.</returns>
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

        /// <summary>
        /// Updates the stock quantity of a specific product by a given amount.
        /// </summary>
        /// <param name="productId">The ID of the product whose stock needs to be updated.</param>
        /// <param name="quantity">The amount to adjust the stock by. Can be positive (restock) or negative (reduce).</param>
        /// <returns>True if stock was updated successfully; otherwise, false.</returns>
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
