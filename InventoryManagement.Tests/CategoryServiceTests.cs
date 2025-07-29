using InventoryManagement.Data.Repositories.Interfaces;
using InventoryManagement.Models;
using InventoryManagement.Services;
using InventoryManagement.Services.Interfaces;
using InventoryManagement.Services.Utility;
using NSubstitute;
using NSubstitute.ExceptionExtensions;

namespace InventoryManagement.Tests
{
    public class CategoryServiceTests
    {
        private readonly ICategoryRepository _categoryRepository;
        private readonly ILoggerService<CategoryService> _logger;
        private readonly CategoryService _service;

        public CategoryServiceTests()
        {
            _categoryRepository = Substitute.For<ICategoryRepository>();
            _logger = Substitute.For<ILoggerService<CategoryService>>();
            _service = new CategoryService(_categoryRepository, _logger);
        }

        #region GetAllCategoriesAsync

        [Fact]
        public async Task GetAllCategoriesAsync_ReturnsAllCategories()
        {
            var expected = new List<Category> { new Category { Id = 1, Name = "Test" } };
            _categoryRepository.GetAllAsync().Returns(expected);

            var result = await _service.GetAllCategoriesAsync();

            Assert.Equal(expected, result);
        }

        [Fact]
        public async Task GetAllCategoriesAsync_ThrowsException_LogsAndWraps()
        {
            var ex = new Exception("DB Error");
            _categoryRepository.GetAllAsync().Throws(ex);

            var resultEx = await Assert.ThrowsAsync<Exception>(() => _service.GetAllCategoriesAsync());

            Assert.Equal("Internal server Error", resultEx.Message);
            _logger.Received(1).LogException("Internal server Error", ex);
        }

        #endregion

        #region GetCategoryByIdAsync

        [Fact]
        public async Task GetCategoryByIdAsync_ReturnsCategory()
        {
            var category = new Category { Id = 1, Name = "Category1" };
            _categoryRepository.GetByIdAsync(1).Returns(category);

            var result = await _service.GetCategoryByIdAsync(1);

            Assert.Equal(category, result);
        }

        [Fact]
        public async Task GetCategoryByIdAsync_ThrowsException_LogsAndWraps()
        {
            var ex = new Exception("Error getting category");
            _categoryRepository.GetByIdAsync(1).Throws(ex);

            var resultEx = await Assert.ThrowsAsync<Exception>(() => _service.GetCategoryByIdAsync(1));

            Assert.Equal("Internal server Error", resultEx.Message);
            _logger.Received(1).LogException("Internal server Error", ex);
        }

        #endregion

        #region CreateCategoryAsync

        [Fact]
        public async Task CreateCategoryAsync_SetsDatesAndReturnsCategory()
        {
            var input = new Category { Name = "New Category" };
            var resultCategory = new Category { Id = 1, Name = "New Category" };

            _categoryRepository.AddAsync(Arg.Any<Category>()).Returns(resultCategory);

            var result = await _service.CreateCategoryAsync(input);

            Assert.Equal(resultCategory, result);
            Assert.True(input.CreatedDate <= DateTime.UtcNow);
            Assert.True(input.UpdatedDate <= DateTime.UtcNow);
        }

        [Fact]
        public async Task CreateCategoryAsync_ThrowsException_LogsAndWraps()
        {
            var input = new Category { Name = "New" };
            var ex = new Exception("Insert failed");

            _categoryRepository.AddAsync(Arg.Any<Category>()).Throws(ex);

            var resultEx = await Assert.ThrowsAsync<Exception>(() => _service.CreateCategoryAsync(input));

            Assert.Equal("Internal server Error", resultEx.Message);
            _logger.Received(1).LogException("Internal server Error", ex);
        }

        #endregion

        #region UpdateCategoryAsync

        [Fact]
        public async Task UpdateCategoryAsync_UpdatesAndReturnsCategory()
        {
            var existing = new Category { Id = 1, Name = "Old" };
            var update = new Category { Name = "New", Description = "Desc" };

            _categoryRepository.GetByIdAsync(1).Returns(existing);
            _categoryRepository.UpdateAsync(Arg.Any<Category>()).Returns(call => call.Arg<Category>());

            var result = await _service.UpdateCategoryAsync(1, update);

            Assert.Equal("New", result.Name);
            Assert.Equal("Desc", result.Description);
        }

        [Fact]
        public async Task UpdateCategoryAsync_CategoryNotFound_ReturnsNull()
        {
            _categoryRepository.GetByIdAsync(1).Returns((Category)null);

            var result = await _service.UpdateCategoryAsync(1, new Category());

            Assert.Null(result);
        }

        [Fact]
        public async Task UpdateCategoryAsync_ThrowsException_LogsAndWraps()
        {
            var ex = new Exception("DB failure");
            _categoryRepository.GetByIdAsync(1).Throws(ex);

            var resultEx = await Assert.ThrowsAsync<Exception>(() => _service.UpdateCategoryAsync(1, new Category()));

            Assert.Equal("Internal server Error", resultEx.Message);
            _logger.Received(1).LogException("Internal server Error", ex);
        }

        #endregion

        #region DeleteCategoryAsync

        [Fact]
        public async Task DeleteCategoryAsync_ReturnsTrueOnSuccess()
        {
            _categoryRepository.DeleteAsync(1).Returns(true);

            var result = await _service.DeleteCategoryAsync(1);

            Assert.True(result);
        }

        [Fact]
        public async Task DeleteCategoryAsync_ThrowsException_LogsAndWraps()
        {
            var ex = new Exception("Delete failed");
            _categoryRepository.DeleteAsync(1).Throws(ex);

            var resultEx = await Assert.ThrowsAsync<Exception>(() => _service.DeleteCategoryAsync(1));

            Assert.Equal("Internal server Error", resultEx.Message);
            _logger.Received(1).LogException("Internal server Error", ex);
        }

        #endregion

        #region GetCategoriesWithProductsAsync

        [Fact]
        public async Task GetCategoriesWithProductsAsync_ReturnsCategoriesWithProducts()
        {
            var categories = new List<Category> { new Category { Id = 1, Name = "Cat1" } };
            _categoryRepository.GetCategoriesWithProductsAsync().Returns(categories);

            var result = await _service.GetCategoriesWithProductsAsync();

            Assert.Equal(categories, result);
        }

        [Fact]
        public async Task GetCategoriesWithProductsAsync_ThrowsException_LogsAndWraps()
        {
            var ex = new Exception("Repo fail");
            _categoryRepository.GetCategoriesWithProductsAsync().Throws(ex);

            var resultEx = await Assert.ThrowsAsync<Exception>(() => _service.GetCategoriesWithProductsAsync());

            Assert.Equal("Internal server Error", resultEx.Message);
            _logger.Received(1).LogException("Internal server Error", ex);
        }

        #endregion
    }
}