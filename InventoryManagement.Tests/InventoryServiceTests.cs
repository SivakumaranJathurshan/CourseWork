using InventoryManagement.Data.Repositories.Interfaces;
using InventoryManagement.Models;
using InventoryManagement.Models.DTO;
using InventoryManagement.Services;
using InventoryManagement.Services.Utility;
using NSubstitute;
using NSubstitute.ExceptionExtensions;

namespace InventoryManagement.Tests
{
    public class InventoryServiceTests
    {
        private readonly IInventoryRepository _inventoryRepository;
        private readonly ILoggerService<InventoryService> _logger;
        private readonly InventoryService _service;

        public InventoryServiceTests()
        {
            _inventoryRepository = Substitute.For<IInventoryRepository>();
            _logger = Substitute.For<ILoggerService<InventoryService>>();
            _service = new InventoryService(_inventoryRepository, _logger);
        }

        #region GetAllInventoryAsync

        [Fact]
        public async Task GetAllInventoryAsync_ReturnsItems()
        {
            var items = new List<InventoryItem> { new InventoryItem { Id = 1, Quantity = 10 } };
            _inventoryRepository.GetAllAsync().Returns(items);

            var result = await _service.GetAllInventoryAsync();

            Assert.Equal(items, result);
        }

        [Fact]
        public async Task GetAllInventoryAsync_ThrowsException_Logs()
        {
            var ex = new Exception("DB Error");
            _inventoryRepository.GetAllAsync().Throws(ex);

            var thrown = await Assert.ThrowsAsync<Exception>(() => _service.GetAllInventoryAsync());

            Assert.Equal("Internal server Error", thrown.Message);
            _logger.Received(1).LogException("Internal server Error", ex);
        }

        #endregion

        #region GetInventoryByIdAsync

        [Fact]
        public async Task GetInventoryByIdAsync_ReturnsItem()
        {
            var item = new InventoryItem { Id = 1 };
            _inventoryRepository.GetByIdAsync(1).Returns(item);

            var result = await _service.GetInventoryByIdAsync(1);

            Assert.Equal(item, result);
        }

        [Fact]
        public async Task GetInventoryByIdAsync_ThrowsException_Logs()
        {
            var ex = new Exception("Error");
            _inventoryRepository.GetByIdAsync(1).Throws(ex);

            var thrown = await Assert.ThrowsAsync<Exception>(() => _service.GetInventoryByIdAsync(1));

            Assert.Equal("Internal server Error", thrown.Message);
            _logger.Received(1).LogException("Internal server Error", ex);
        }

        #endregion

        #region CreateInventoryItemAsync

        [Fact]
        public async Task CreateInventoryItemAsync_ReturnsCreatedItem()
        {
            var input = new InventoryItemCreateDTO(1, 5, 2, 50);
            var output = new InventoryItem { Id = 1, ProductId = 1, Quantity = 5, MinimumStock = 2, MaximumStock = 50 };

            InventoryItem capturedInventoryItem = null;
            _inventoryRepository.AddAsync(Arg.Do<InventoryItem>(inventoryItem => capturedInventoryItem = inventoryItem)).Returns(output);

            var result = await _service.CreateInventoryItemAsync(input);

            Assert.Equal(output, result);
        }

        [Fact]
        public async Task CreateInventoryItemAsync_ThrowsException_Logs()
        {
            var input = new InventoryItemCreateDTO(1, 5, 2, 50);
            var ex = new Exception("Add failed");

            _inventoryRepository.AddAsync(Arg.Any<InventoryItem>()).Throws(ex);

            var thrown = await Assert.ThrowsAsync<Exception>(() => _service.CreateInventoryItemAsync(input));

            Assert.Equal("Internal server Error", thrown.Message);
            _logger.Received(1).LogException("Internal server Error", ex);
        }

        #endregion

        #region UpdateInventoryItemAsync

        [Fact]
        public async Task UpdateInventoryItemAsync_UpdatesAndReturnsItem()
        {
            var existing = new InventoryItem { Id = 1, Quantity = 5, MinimumStock = 1, MaximumStock = 10 };
            var update = new InventoryItemUpdateDTO(1, 1, 10, 2, 20);

            _inventoryRepository.GetByIdAsync(1).Returns(existing);
            _inventoryRepository.UpdateAsync(Arg.Any<InventoryItem>()).Returns(call => call.Arg<InventoryItem>());

            var result = await _service.UpdateInventoryItemAsync(1, update);

            Assert.Equal(10, result.Quantity);
            Assert.Equal(2, result.MinimumStock);
            Assert.Equal(20, result.MaximumStock);
            Assert.True(result.UpdatedDate <= DateTime.UtcNow);
        }

        [Fact]
        public async Task UpdateInventoryItemAsync_ItemNotFound_ReturnsNull()
        {
            var update = new InventoryItemUpdateDTO(1, 1, 10, 1, 20);
            _inventoryRepository.GetByIdAsync(1).Returns((InventoryItem)null);

            var result = await _service.UpdateInventoryItemAsync(1, update);

            Assert.Null(result);
        }

        [Fact]
        public async Task UpdateInventoryItemAsync_ThrowsException_Logs()
        {
            var update = new InventoryItemUpdateDTO(1, 1, 10, 1, 20);
            var ex = new Exception("Error");
            _inventoryRepository.GetByIdAsync(1).Throws(ex);

            var thrown = await Assert.ThrowsAsync<Exception>(() => _service.UpdateInventoryItemAsync(1, update));

            Assert.Equal("Internal server Error", thrown.Message);
            _logger.Received(1).LogException("Internal server Error", ex);
        }

        #endregion

        #region DeleteInventoryItemAsync

        [Fact]
        public async Task DeleteInventoryItemAsync_ReturnsTrue()
        {
            _inventoryRepository.DeleteAsync(1).Returns(true);

            var result = await _service.DeleteInventoryItemAsync(1);

            Assert.True(result);
        }

        [Fact]
        public async Task DeleteInventoryItemAsync_ThrowsException_Logs()
        {
            var ex = new Exception("Delete error");
            _inventoryRepository.DeleteAsync(1).Throws(ex);

            var thrown = await Assert.ThrowsAsync<Exception>(() => _service.DeleteInventoryItemAsync(1));

            Assert.Equal("Internal server Error", thrown.Message);
            _logger.Received(1).LogException("Internal server Error", ex);
        }

        #endregion

        #region GetInventoryWithProductsAsync

        [Fact]
        public async Task GetInventoryWithProductsAsync_ReturnsItems()
        {
            var items = new List<InventoryItem> { new InventoryItem { Id = 1 } };
            _inventoryRepository.GetInventoryWithProductsAsync().Returns(items);

            var result = await _service.GetInventoryWithProductsAsync();

            Assert.Equal(items, result);
        }

        [Fact]
        public async Task GetInventoryWithProductsAsync_ThrowsException_Logs()
        {
            var ex = new Exception("Failed");
            _inventoryRepository.GetInventoryWithProductsAsync().Throws(ex);

            var thrown = await Assert.ThrowsAsync<Exception>(() => _service.GetInventoryWithProductsAsync());

            Assert.Equal("Internal server Error", thrown.Message);
            _logger.Received(1).LogException("Internal server Error", ex);
        }

        #endregion

        #region GetInventoryByProductIdAsync

        [Fact]
        public async Task GetInventoryByProductIdAsync_ReturnsItem()
        {
            var item = new InventoryItem { Id = 1, ProductId = 5 };
            _inventoryRepository.GetInventoryByProductIdAsync(5).Returns(item);

            var result = await _service.GetInventoryByProductIdAsync(5);

            Assert.Equal(item, result);
        }

        [Fact]
        public async Task GetInventoryByProductIdAsync_ThrowsException_Logs()
        {
            var ex = new Exception("Error");
            _inventoryRepository.GetInventoryByProductIdAsync(5).Throws(ex);

            var thrown = await Assert.ThrowsAsync<Exception>(() => _service.GetInventoryByProductIdAsync(5));

            Assert.Equal("Internal server Error", thrown.Message);
            _logger.Received(1).LogException("Internal server Error", ex);
        }

        #endregion

        #region GetLowStockItemsAsync

        [Fact]
        public async Task GetLowStockItemsAsync_ReturnsItems()
        {
            var items = new List<InventoryItem> { new InventoryItem { Id = 1 } };
            _inventoryRepository.GetLowStockItemsAsync().Returns(items);

            var result = await _service.GetLowStockItemsAsync();

            Assert.Equal(items, result);
        }

        [Fact]
        public async Task GetLowStockItemsAsync_ThrowsException_Logs()
        {
            var ex = new Exception("Error");
            _inventoryRepository.GetLowStockItemsAsync().Throws(ex);

            var thrown = await Assert.ThrowsAsync<Exception>(() => _service.GetLowStockItemsAsync());

            Assert.Equal("Internal server Error", thrown.Message);
            _logger.Received(1).LogException("Internal server Error", ex);
        }

        #endregion

        #region UpdateStockAsync

        [Fact]
        public async Task UpdateStockAsync_UpdatesStock_ReturnsTrue()
        {
            var item = new InventoryItem { Id = 1, Quantity = 5 };
            _inventoryRepository.GetInventoryByProductIdAsync(5).Returns(item);

            var result = await _service.UpdateStockAsync(5, 10);

            Assert.True(result);
            Assert.Equal(15, item.Quantity);
        }

        [Fact]
        public async Task UpdateStockAsync_ItemNotFound_ReturnsFalse()
        {
            _inventoryRepository.GetInventoryByProductIdAsync(5).Returns((InventoryItem)null);

            var result = await _service.UpdateStockAsync(5, 10);

            Assert.False(result);
        }

        [Fact]
        public async Task UpdateStockAsync_ThrowsException_Logs()
        {
            var ex = new Exception("Update error");
            _inventoryRepository.GetInventoryByProductIdAsync(5).Throws(ex);

            var thrown = await Assert.ThrowsAsync<Exception>(() => _service.UpdateStockAsync(5, 10));

            Assert.Equal("Internal server Error", thrown.Message);
            _logger.Received(1).LogException("Internal server Error", ex);
        }

        #endregion
    }
}
