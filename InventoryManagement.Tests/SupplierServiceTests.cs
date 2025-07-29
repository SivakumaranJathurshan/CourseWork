

using InventoryManagement.Data.Repositories.Interfaces;
using InventoryManagement.Models;
using InventoryManagement.Services;
using InventoryManagement.Services.Utility;
using NSubstitute;
using NSubstitute.ExceptionExtensions;

namespace InventoryManagement.Tests
{
    public class SupplierServiceTests
    {
        private readonly ISupplierRepository _supplierRepository;
        private readonly ILoggerService<SupplierService> _logger;
        private readonly SupplierService _service;

        public SupplierServiceTests()
        {
            _supplierRepository = Substitute.For<ISupplierRepository>();
            _logger = Substitute.For<ILoggerService<SupplierService>>();
            _service = new SupplierService(_supplierRepository, _logger);
        }

        #region GetAllSuppliersAsync

        [Fact]
        public async Task GetAllSuppliersAsync_ShouldReturnSuppliers_WhenSuccess()
        {
            // Arrange
            var expected = new List<Supplier> { new Supplier { Id = 1, Name = "Supplier 1" } };
            _supplierRepository.GetAllAsync().Returns(expected);

            // Act
            var result = await _service.GetAllSuppliersAsync();

            // Assert
            Assert.Equal(expected, result);
        }

        [Fact]
        public async Task GetAllSuppliersAsync_ShouldLogAndThrow_WhenExceptionOccurs()
        {
            // Arrange
            var ex = new Exception("DB error");
            _supplierRepository.GetAllAsync().Throws(ex);

            // Act & Assert
            var result = await Assert.ThrowsAsync<Exception>(() => _service.GetAllSuppliersAsync());
            Assert.Equal("Internal server Error", result.Message);
            _logger.Received(1).LogException("Internal server Error", ex);
        }

        #endregion

        #region GetSupplierByIdAsync

        [Fact]
        public async Task GetSupplierByIdAsync_ShouldReturnSupplier_WhenFound()
        {
            var supplier = new Supplier { Id = 2, Name = "Test" };
            _supplierRepository.GetByIdAsync(2).Returns(supplier);

            var result = await _service.GetSupplierByIdAsync(2);

            Assert.Equal(supplier, result);
        }

        [Fact]
        public async Task GetSupplierByIdAsync_ShouldLogAndThrow_WhenExceptionOccurs()
        {
            var ex = new Exception("DB failure");
            _supplierRepository.GetByIdAsync(Arg.Any<int>()).Throws(ex);

            var result = await Assert.ThrowsAsync<Exception>(() => _service.GetSupplierByIdAsync(1));
            Assert.Equal("Internal server Error", result.Message);
            _logger.Received(1).LogException("Internal server Error", ex);
        }

        #endregion

        #region CreateSupplierAsync

        [Fact]
        public async Task CreateSupplierAsync_ShouldReturnCreatedSupplier()
        {
            var supplier = new Supplier { Name = "New Supplier" };
            _supplierRepository.AddAsync(Arg.Any<Supplier>()).Returns(callInfo => callInfo.Arg<Supplier>());

            var result = await _service.CreateSupplierAsync(supplier);

            Assert.Equal(supplier.Name, result.Name);
            Assert.True(result.CreatedDate <= DateTime.UtcNow);
            Assert.True(result.UpdatedDate <= DateTime.UtcNow);
        }

        [Fact]
        public async Task CreateSupplierAsync_ShouldLogAndThrow_WhenExceptionOccurs()
        {
            var ex = new Exception("Insert failed");
            _supplierRepository.AddAsync(Arg.Any<Supplier>()).Throws(ex);

            var result = await Assert.ThrowsAsync<Exception>(() => _service.CreateSupplierAsync(new Supplier()));
            Assert.Equal("Internal server Error", result.Message);
            _logger.Received(1).LogException("Internal server Error", ex);
        }

        #endregion

        #region UpdateSupplierAsync

        [Fact]
        public async Task UpdateSupplierAsync_ShouldUpdateAndReturnSupplier_WhenFound()
        {
            var supplier = new Supplier { Name = "Updated", ContactPerson = "Jane" };
            var existing = new Supplier { Id = 1, Name = "Old", ContactPerson = "John" };

            _supplierRepository.GetByIdAsync(1).Returns(existing);
            _supplierRepository.UpdateAsync(Arg.Any<Supplier>()).Returns(call => call.Arg<Supplier>());

            var result = await _service.UpdateSupplierAsync(1, supplier);

            Assert.Equal("Updated", result.Name);
            Assert.Equal("Jane", result.ContactPerson);
            Assert.True(result.UpdatedDate <= DateTime.UtcNow);
        }

        [Fact]
        public async Task UpdateSupplierAsync_ShouldReturnNull_WhenNotFound()
        {
            _supplierRepository.GetByIdAsync(1).Returns((Supplier)null);

            var result = await _service.UpdateSupplierAsync(1, new Supplier());

            Assert.Null(result);
        }

        [Fact]
        public async Task UpdateSupplierAsync_ShouldLogAndThrow_WhenExceptionOccurs()
        {
            var ex = new Exception("Update failed");
            _supplierRepository.GetByIdAsync(Arg.Any<int>()).Throws(ex);

            var result = await Assert.ThrowsAsync<Exception>(() => _service.UpdateSupplierAsync(1, new Supplier()));
            Assert.Equal("Internal server Error", result.Message);
            _logger.Received(1).LogException("Internal server Error", ex);
        }

        #endregion

        #region DeleteSupplierAsync

        [Fact]
        public async Task DeleteSupplierAsync_ShouldReturnTrue_WhenDeleted()
        {
            _supplierRepository.DeleteAsync(1).Returns(true);

            var result = await _service.DeleteSupplierAsync(1);

            Assert.True(result);
        }

        [Fact]
        public async Task DeleteSupplierAsync_ShouldLogAndThrow_WhenExceptionOccurs()
        {
            var ex = new Exception("Delete failed");
            _supplierRepository.DeleteAsync(1).Throws(ex);

            var result = await Assert.ThrowsAsync<Exception>(() => _service.DeleteSupplierAsync(1));
            Assert.Equal("Internal server Error", result.Message);
            _logger.Received(1).LogException("Internal server Error", ex);
        }

        #endregion

        #region GetSuppliersWithProductsAsync

        [Fact]
        public async Task GetSuppliersWithProductsAsync_ShouldReturnSuppliersWithProducts()
        {
            var suppliers = new List<Supplier>
            {
                new Supplier { Id = 1, Name = "Supp 1", Products = new List<Product> { new Product() } }
            };
            _supplierRepository.GetSuppliersWithProductsAsync().Returns(suppliers);

            var result = await _service.GetSuppliersWithProductsAsync();

            Assert.Single(result);
            Assert.NotEmpty(result.First().Products);
        }

        [Fact]
        public async Task GetSuppliersWithProductsAsync_ShouldLogAndThrow_WhenExceptionOccurs()
        {
            var ex = new Exception("Join failed");
            _supplierRepository.GetSuppliersWithProductsAsync().Throws(ex);

            var result = await Assert.ThrowsAsync<Exception>(() => _service.GetSuppliersWithProductsAsync());
            Assert.Equal("Internal server Error", result.Message);
            _logger.Received(1).LogException("Internal server Error", ex);
        }

        #endregion
    }
}
