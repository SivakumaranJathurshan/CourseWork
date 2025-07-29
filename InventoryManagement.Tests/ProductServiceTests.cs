using InventoryManagement.Data.Repositories.Interfaces;
using InventoryManagement.Models;
using InventoryManagement.Services;
using InventoryManagement.Services.Utility;
using NSubstitute;
using NSubstitute.ExceptionExtensions;

namespace InventoryManagement.Tests
{
    public class ProductServiceTests
    {
        private readonly IProductRepository _productRepository;
        private readonly ILoggerService<ProductService> _logger;
        private readonly ProductService _service;

        public ProductServiceTests()
        {
            _productRepository = Substitute.For<IProductRepository>();
            _logger = Substitute.For<ILoggerService<ProductService>>();
            _service = new ProductService(_productRepository, _logger);
        }

        #region GetAllProductsAsync

        [Fact]
        public async Task GetAllProductsAsync_ReturnsAllProducts()
        {
            var products = new List<Product> { new Product { Id = 1, Name = "Test" } };
            _productRepository.GetAllAsync().Returns(products);

            var result = await _service.GetAllProductsAsync();

            Assert.Equal(products, result);
        }

        [Fact]
        public async Task GetAllProductsAsync_ThrowsException_LogsAndRethrows()
        {
            var ex = new Exception("DB Error");
            _productRepository.GetAllAsync().Throws(ex);

            var thrown = await Assert.ThrowsAsync<Exception>(() => _service.GetAllProductsAsync());

            Assert.Equal("Internal server Error", thrown.Message);
            _logger.Received(1).LogException("Internal server Error", ex);
        }

        #endregion

        #region GetProductByIdAsync

        [Fact]
        public async Task GetProductByIdAsync_ReturnsProductWithDetails()
        {
            var product = new Product { Id = 1, Name = "Product A" };
            _productRepository.GetProductWithDetailsAsync(1).Returns(product);

            var result = await _service.GetProductByIdAsync(1);

            Assert.Equal(product, result);
        }

        [Fact]
        public async Task GetProductByIdAsync_ThrowsException_LogsAndRethrows()
        {
            var ex = new Exception("Error");
            _productRepository.GetProductWithDetailsAsync(1).Throws(ex);

            var thrown = await Assert.ThrowsAsync<Exception>(() => _service.GetProductByIdAsync(1));

            Assert.Equal("Internal server Error", thrown.Message);
            _logger.Received(1).LogException("Internal server Error", ex);
        }

        #endregion

        #region CreateProductAsync

        [Fact]
        public async Task CreateProductAsync_SetsDates_And_ReturnsProduct()
        {
            var input = new Product { Name = "New", SKU = "ABC123" };
            var saved = new Product { Id = 1, Name = "New", SKU = "ABC123" };

            _productRepository.AddAsync(Arg.Any<Product>()).Returns(saved);

            var result = await _service.CreateProductAsync(input);

            Assert.Equal(saved, result);
            Assert.True(input.CreatedDate <= DateTime.UtcNow);
            Assert.True(input.UpdatedDate <= DateTime.UtcNow);
        }

        [Fact]
        public async Task CreateProductAsync_ThrowsException_LogsAndRethrows()
        {
            var input = new Product { Name = "New", SKU = "ABC123" };
            var ex = new Exception("Create failed");

            _productRepository.AddAsync(Arg.Any<Product>()).Throws(ex);

            var thrown = await Assert.ThrowsAsync<Exception>(() => _service.CreateProductAsync(input));

            Assert.Equal("Internal server Error", thrown.Message);
            _logger.Received(1).LogException("Internal server Error", ex);
        }

        #endregion

        #region UpdateProductAsync

        [Fact]
        public async Task UpdateProductAsync_UpdatesAndReturnsProduct()
        {
            var existing = new Product { Id = 1, Name = "Old", SKU = "SKU1" };
            var update = new Product
            {
                Name = "New",
                Description = "Updated",
                SKU = "SKU2",
                Price = 10.5m,
                CategoryId = 2,
                SupplierId = 3
            };

            _productRepository.GetByIdAsync(1).Returns(existing);
            _productRepository.UpdateAsync(Arg.Any<Product>()).Returns(call => call.Arg<Product>());

            var result = await _service.UpdateProductAsync(1, update);

            Assert.Equal("New", result.Name);
            Assert.Equal("Updated", result.Description);
            Assert.Equal("SKU2", result.SKU);
            Assert.Equal(10.5m, result.Price);
            Assert.Equal(2, result.CategoryId);
            Assert.Equal(3, result.SupplierId);
        }

        [Fact]
        public async Task UpdateProductAsync_ProductNotFound_ReturnsNull()
        {
            _productRepository.GetByIdAsync(1).Returns((Product)null);

            var result = await _service.UpdateProductAsync(1, new Product());

            Assert.Null(result);
        }

        [Fact]
        public async Task UpdateProductAsync_ThrowsException_LogsAndRethrows()
        {
            var ex = new Exception("Update failed");
            _productRepository.GetByIdAsync(1).Throws(ex);

            var thrown = await Assert.ThrowsAsync<Exception>(() => _service.UpdateProductAsync(1, new Product()));

            Assert.Equal("Internal server Error", thrown.Message);
            _logger.Received(1).LogException("Internal server Error", ex);
        }

        #endregion

        #region DeleteProductAsync

        [Fact]
        public async Task DeleteProductAsync_ReturnsTrue()
        {
            _productRepository.DeleteAsync(1).Returns(true);

            var result = await _service.DeleteProductAsync(1);

            Assert.True(result);
        }

        [Fact]
        public async Task DeleteProductAsync_ThrowsException_LogsAndRethrows()
        {
            var ex = new Exception("Delete failed");
            _productRepository.DeleteAsync(1).Throws(ex);

            var thrown = await Assert.ThrowsAsync<Exception>(() => _service.DeleteProductAsync(1));

            Assert.Equal("Internal server Error", thrown.Message);
            _logger.Received(1).LogException("Internal server Error", ex);
        }

        #endregion

        #region GetProductsWithDetailsAsync

        [Fact]
        public async Task GetProductsWithDetailsAsync_ReturnsProducts()
        {
            var products = new List<Product> { new Product { Id = 1 } };
            _productRepository.GetProductsWithDetailsAsync().Returns(products);

            var result = await _service.GetProductsWithDetailsAsync();

            Assert.Equal(products, result);
        }

        [Fact]
        public async Task GetProductsWithDetailsAsync_ThrowsException_LogsAndRethrows()
        {
            var ex = new Exception("Error");
            _productRepository.GetProductsWithDetailsAsync().Throws(ex);

            var thrown = await Assert.ThrowsAsync<Exception>(() => _service.GetProductsWithDetailsAsync());

            Assert.Equal("Internal server Error", thrown.Message);
            _logger.Received(1).LogException("Internal server Error", ex);
        }

        #endregion

        #region GetProductsByCategoryAsync

        [Fact]
        public async Task GetProductsByCategoryAsync_ReturnsProducts()
        {
            var products = new List<Product> { new Product { Id = 1, CategoryId = 2 } };
            _productRepository.GetProductsByCategoryAsync(2).Returns(products);

            var result = await _service.GetProductsByCategoryAsync(2);

            Assert.Equal(products, result);
        }

        [Fact]
        public async Task GetProductsByCategoryAsync_ThrowsException_LogsAndRethrows()
        {
            var ex = new Exception("Error");
            _productRepository.GetProductsByCategoryAsync(2).Throws(ex);

            var thrown = await Assert.ThrowsAsync<Exception>(() => _service.GetProductsByCategoryAsync(2));

            Assert.Equal("Internal server Error", thrown.Message);
            _logger.Received(1).LogException("Internal server Error", ex);
        }

        #endregion

        #region GetProductBySkuAsync

        [Fact]
        public async Task GetProductBySkuAsync_ReturnsProduct()
        {
            var product = new Product { Id = 1, SKU = "SKU001" };
            _productRepository.GetProductBySkuAsync("SKU001").Returns(product);

            var result = await _service.GetProductBySkuAsync("SKU001");

            Assert.Equal(product, result);
        }

        [Fact]
        public async Task GetProductBySkuAsync_ThrowsException_LogsAndRethrows()
        {
            var ex = new Exception("Lookup failed");
            _productRepository.GetProductBySkuAsync("SKU001").Throws(ex);

            var thrown = await Assert.ThrowsAsync<Exception>(() => _service.GetProductBySkuAsync("SKU001"));

            Assert.Equal("Internal server Error", thrown.Message);
            _logger.Received(1).LogException("Internal server Error", ex);
        }

        #endregion
    }
}
