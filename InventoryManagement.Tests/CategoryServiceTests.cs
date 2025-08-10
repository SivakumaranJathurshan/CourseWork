using InventoryManagement.Data.Repositories.Interfaces;
using InventoryManagement.Models;
using InventoryManagement.Models.DTO;
using InventoryManagement.Services;
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
            var input = new CategoryCreateDTO("New Category", "Test Description");
            var resultCategory = new Category { Id = 1, Name = "New Category", Description = "Test Description" };

            _categoryRepository.AddAsync(Arg.Any<Category>()).Returns(resultCategory);

            var result = await _service.CreateCategoryAsync(input);

            Assert.Equal(resultCategory, result);

            // Verify that AddAsync was called with a Category that has the correct properties
            await _categoryRepository.Received(1).AddAsync(Arg.Is<Category>(c =>
                c.Name == input.Name &&
                c.Description == input.Description &&
                c.CreatedDate <= DateTime.UtcNow &&
                c.UpdatedDate <= DateTime.UtcNow));
        }

        [Fact]
        public async Task CreateCategoryAsync_ThrowsException_LogsAndWraps()
        {
            var input = new CategoryCreateDTO("New", "Test");
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
            var existing = new Category { Id = 1, Name = "Old", Description = "Old Desc" };
            var update = new CategoryUpdateDTO(1, "New", "New Desc");

            _categoryRepository.GetByIdAsync(1).Returns(existing);
            _categoryRepository.UpdateAsync(Arg.Any<Category>()).Returns(call => call.Arg<Category>());

            var result = await _service.UpdateCategoryAsync(1, update);

            Assert.Equal("New", result.Name);
            Assert.Equal("New Desc", result.Description);
            Assert.True(result.UpdatedDate <= DateTime.UtcNow);
        }

        [Fact]
        public async Task UpdateCategoryAsync_CategoryNotFound_ReturnsNull()
        {
            var update = new CategoryUpdateDTO(1, "New", "New Desc");
            _categoryRepository.GetByIdAsync(1).Returns((Category)null);

            var result = await _service.UpdateCategoryAsync(1, update);

            Assert.Null(result);
        }

        [Fact]
        public async Task UpdateCategoryAsync_ThrowsException_LogsAndWraps()
        {
            var update = new CategoryUpdateDTO(1, "New", "New Desc");
            var ex = new Exception("DB failure");
            _categoryRepository.GetByIdAsync(1).Throws(ex);

            var resultEx = await Assert.ThrowsAsync<Exception>(() => _service.UpdateCategoryAsync(1, update));

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