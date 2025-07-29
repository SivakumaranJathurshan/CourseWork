using InventoryManagement.Data.Repositories.Interfaces;
using InventoryManagement.Models;
using InventoryManagement.Services;
using InventoryManagement.Services.Interfaces;
using NSubstitute;

namespace InventoryManagement.Tests
{
    public class CategoryServiceTests
    {
        private readonly ICategoryRepository _categoryRepository;
        private readonly CategoryService _categoryService;

        public CategoryServiceTests()
        {
            _categoryRepository = Substitute.For<ICategoryRepository>();
            _categoryService = new CategoryService(_categoryRepository);
        }

        #region GetAllCategoriesAsync Tests

        [Fact]
        public async Task GetAllCategoriesAsync_ShouldReturnAllCategories_WhenCalled()
        {
            // Arrange
            var expectedCategories = new List<Category>
            {
                new Category { Id = 1, Name = "Electronics", Description = "Electronic items" },
                new Category { Id = 2, Name = "Clothing", Description = "Apparel items" }
            };

            _categoryRepository.GetAllAsync().Returns(expectedCategories);

            // Act
            var result = await _categoryService.GetAllCategoriesAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count());
            Assert.Equal(expectedCategories.First().Id, result.First().Id);
            Assert.Equal(expectedCategories.First().Name, result.First().Name);
            Assert.Equal(expectedCategories.Last().Id, result.Last().Id);
            Assert.Equal(expectedCategories.Last().Name, result.Last().Name);
            await _categoryRepository.Received(1).GetAllAsync();
        }

        [Fact]
        public async Task GetAllCategoriesAsync_ShouldReturnEmptyList_WhenNoCategoriesExist()
        {
            // Arrange
            var expectedCategories = new List<Category>();
            _categoryRepository.GetAllAsync().Returns(expectedCategories);

            // Act
            var result = await _categoryService.GetAllCategoriesAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
            await _categoryRepository.Received(1).GetAllAsync();
        }

        #endregion

        #region GetCategoryByIdAsync Tests

        [Fact]
        public async Task GetCategoryByIdAsync_ShouldReturnCategory_WhenCategoryExists()
        {
            // Arrange
            var categoryId = 1;
            var expectedCategory = new Category
            {
                Id = categoryId,
                Name = "Electronics",
                Description = "Electronic items"
            };

            _categoryRepository.GetByIdAsync(categoryId).Returns(expectedCategory);

            // Act
            var result = await _categoryService.GetCategoryByIdAsync(categoryId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(expectedCategory.Id, result.Id);
            Assert.Equal(expectedCategory.Name, result.Name);
            Assert.Equal(expectedCategory.Description, result.Description);
            await _categoryRepository.Received(1).GetByIdAsync(categoryId);
        }

        [Fact]
        public async Task GetCategoryByIdAsync_ShouldReturnNull_WhenCategoryDoesNotExist()
        {
            // Arrange
            var categoryId = 999;
            _categoryRepository.GetByIdAsync(categoryId).Returns((Category)null);

            // Act
            var result = await _categoryService.GetCategoryByIdAsync(categoryId);

            // Assert
            Assert.Null(result);
            await _categoryRepository.Received(1).GetByIdAsync(categoryId);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        [InlineData(-999)]
        public async Task GetCategoryByIdAsync_ShouldHandleInvalidIds(int invalidId)
        {
            // Arrange
            _categoryRepository.GetByIdAsync(invalidId).Returns((Category)null);

            // Act
            var result = await _categoryService.GetCategoryByIdAsync(invalidId);

            // Assert
            Assert.Null(result);
            await _categoryRepository.Received(1).GetByIdAsync(invalidId);
        }

        #endregion

        #region CreateCategoryAsync Tests

        [Fact]
        public async Task CreateCategoryAsync_ShouldCreateCategory_WhenValidCategoryProvided()
        {
            // Arrange
            var newCategory = new Category
            {
                Name = "Books",
                Description = "Book items"
            };

            var createdCategory = new Category
            {
                Id = 1,
                Name = "Books",
                Description = "Book items",
                CreatedDate = DateTime.UtcNow,
                UpdatedDate = DateTime.UtcNow
            };

            _categoryRepository.AddAsync(Arg.Any<Category>()).Returns(createdCategory);

            // Act
            var result = await _categoryService.CreateCategoryAsync(newCategory);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(1, result.Id);
            Assert.Equal("Books", result.Name);
            Assert.Equal("Book items", result.Description);

            // Verify dates were set (within 1 second tolerance)
            var timeDifference = Math.Abs((DateTime.UtcNow - newCategory.CreatedDate).TotalSeconds);
            Assert.True(timeDifference < 1, "CreatedDate should be set to current UTC time");

            timeDifference = Math.Abs((DateTime.UtcNow - newCategory.UpdatedDate).TotalSeconds);
            Assert.True(timeDifference < 1, "UpdatedDate should be set to current UTC time");

            await _categoryRepository.Received(1).AddAsync(Arg.Is<Category>(c =>
                c.Name == "Books" &&
                c.Description == "Book items"));
        }

        [Fact]
        public async Task CreateCategoryAsync_ShouldSetCreatedAndUpdatedDates_WhenCreatingCategory()
        {
            // Arrange
            var newCategory = new Category
            {
                Name = "Sports",
                Description = "Sports equipment"
            };

            var beforeCreation = DateTime.UtcNow;

            _categoryRepository.AddAsync(Arg.Any<Category>()).Returns(newCategory);

            // Act
            await _categoryService.CreateCategoryAsync(newCategory);

            var afterCreation = DateTime.UtcNow;

            // Assert
            Assert.True(newCategory.CreatedDate >= beforeCreation);
            Assert.True(newCategory.CreatedDate <= afterCreation);
            Assert.True(newCategory.UpdatedDate >= beforeCreation);
            Assert.True(newCategory.UpdatedDate <= afterCreation);
        }

        [Fact]
        public async Task CreateCategoryAsync_ShouldHandleNullCategory_ThrowsException()
        {
            // Arrange
            Category nullCategory = null;

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentNullException>(() =>
                _categoryService.CreateCategoryAsync(nullCategory));
        }

        #endregion

        #region UpdateCategoryAsync Tests

        [Fact]
        public async Task UpdateCategoryAsync_ShouldUpdateCategory_WhenCategoryExists()
        {
            // Arrange
            var categoryId = 1;
            var existingCategory = new Category
            {
                Id = categoryId,
                Name = "Old Electronics",
                Description = "Old description",
                CreatedDate = DateTime.UtcNow.AddDays(-1),
                UpdatedDate = DateTime.UtcNow.AddDays(-1)
            };

            var updateCategory = new Category
            {
                Name = "New Electronics",
                Description = "New description"
            };

            _categoryRepository.GetByIdAsync(categoryId).Returns(existingCategory);
            _categoryRepository.UpdateAsync(Arg.Any<Category>()).Returns(existingCategory);

            var beforeUpdate = DateTime.UtcNow;

            // Act
            var result = await _categoryService.UpdateCategoryAsync(categoryId, updateCategory);

            var afterUpdate = DateTime.UtcNow;

            // Assert
            Assert.NotNull(result);
            Assert.Equal("New Electronics", result.Name);
            Assert.Equal("New description", result.Description);
            Assert.True(result.UpdatedDate >= beforeUpdate);
            Assert.True(result.UpdatedDate <= afterUpdate);

            await _categoryRepository.Received(1).GetByIdAsync(categoryId);
            await _categoryRepository.Received(1).UpdateAsync(existingCategory);
        }

        [Fact]
        public async Task UpdateCategoryAsync_ShouldReturnNull_WhenCategoryDoesNotExist()
        {
            // Arrange
            var categoryId = 999;
            var updateCategory = new Category
            {
                Name = "Non-existent Category",
                Description = "This won't work"
            };

            _categoryRepository.GetByIdAsync(categoryId).Returns((Category)null);

            // Act
            var result = await _categoryService.UpdateCategoryAsync(categoryId, updateCategory);

            // Assert
            Assert.Null(result);
            await _categoryRepository.Received(1).GetByIdAsync(categoryId);
            await _categoryRepository.DidNotReceive().UpdateAsync(Arg.Any<Category>());
        }

        [Fact]
        public async Task UpdateCategoryAsync_ShouldPreserveCreatedDate_WhenUpdating()
        {
            // Arrange
            var categoryId = 1;
            var originalCreatedDate = DateTime.UtcNow.AddDays(-5);
            var originalUpdatedDate = DateTime.UtcNow.AddDays(-3);

            var existingCategory = new Category
            {
                Id = categoryId,
                Name = "Original Name",
                Description = "Original description",
                CreatedDate = originalCreatedDate,
                UpdatedDate = originalUpdatedDate
            };

            var updateCategory = new Category
            {
                Name = "Updated Name",
                Description = "Updated description"
            };

            _categoryRepository.GetByIdAsync(categoryId).Returns(existingCategory);
            _categoryRepository.UpdateAsync(existingCategory).Returns(existingCategory);

            // Act
            var result = await _categoryService.UpdateCategoryAsync(categoryId, updateCategory);

            // Assert
            Assert.Equal(originalCreatedDate, result.CreatedDate);
            Assert.NotEqual(originalUpdatedDate, result.UpdatedDate);

            var timeDifference = Math.Abs((DateTime.UtcNow - result.UpdatedDate).TotalSeconds);
            Assert.True(timeDifference < 1, "UpdatedDate should be set to current UTC time");
        }

        #endregion

        #region DeleteCategoryAsync Tests

        [Fact]
        public async Task DeleteCategoryAsync_ShouldReturnTrue_WhenCategoryDeletedSuccessfully()
        {
            // Arrange
            var categoryId = 1;
            _categoryRepository.DeleteAsync(categoryId).Returns(true);

            // Act
            var result = await _categoryService.DeleteCategoryAsync(categoryId);

            // Assert
            Assert.True(result);
            await _categoryRepository.Received(1).DeleteAsync(categoryId);
        }

        [Fact]
        public async Task DeleteCategoryAsync_ShouldReturnFalse_WhenCategoryDoesNotExist()
        {
            // Arrange
            var categoryId = 999;
            _categoryRepository.DeleteAsync(categoryId).Returns(false);

            // Act
            var result = await _categoryService.DeleteCategoryAsync(categoryId);

            // Assert
            Assert.False(result);
            await _categoryRepository.Received(1).DeleteAsync(categoryId);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        [InlineData(-999)]
        public async Task DeleteCategoryAsync_ShouldReturnFalse_WhenInvalidIdProvided(int invalidId)
        {
            // Arrange
            _categoryRepository.DeleteAsync(invalidId).Returns(false);

            // Act
            var result = await _categoryService.DeleteCategoryAsync(invalidId);

            // Assert
            Assert.False(result);
            await _categoryRepository.Received(1).DeleteAsync(invalidId);
        }

        #endregion

        #region GetCategoriesWithProductsAsync Tests

        [Fact]
        public async Task GetCategoriesWithProductsAsync_ShouldReturnCategoriesWithProducts_WhenCalled()
        {
            // Arrange
            var expectedCategories = new List<Category>
            {
                new Category
                {
                    Id = 1,
                    Name = "Electronics",
                    Description = "Electronic items",
                    Products = new List<Product>
                    {
                        new Product { Id = 1, Name = "Laptop" },
                        new Product { Id = 2, Name = "Phone" }
                    }
                },
                new Category
                {
                    Id = 2,
                    Name = "Clothing",
                    Description = "Apparel items",
                    Products = new List<Product>
                    {
                        new Product { Id = 3, Name = "T-Shirt" }
                    }
                }
            };

            _categoryRepository.GetCategoriesWithProductsAsync().Returns(expectedCategories);

            // Act
            var result = await _categoryService.GetCategoriesWithProductsAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count());
            Assert.Equal(2, result.First().Products.Count);
            Assert.Equal(1, result.Last().Products.Count);
            Assert.Equal("Laptop", result.First().Products.First().Name);
            Assert.Equal("T-Shirt", result.Last().Products.First().Name);
            await _categoryRepository.Received(1).GetCategoriesWithProductsAsync();
        }

        [Fact]
        public async Task GetCategoriesWithProductsAsync_ShouldReturnEmptyList_WhenNoCategoriesExist()
        {
            // Arrange
            var expectedCategories = new List<Category>();
            _categoryRepository.GetCategoriesWithProductsAsync().Returns(expectedCategories);

            // Act
            var result = await _categoryService.GetCategoriesWithProductsAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
            await _categoryRepository.Received(1).GetCategoriesWithProductsAsync();
        }

        #endregion

        #region Integration-style Tests

        [Fact]
        public async Task CategoryService_ShouldHandleCompleteWorkflow()
        {
            // Arrange
            var newCategory = new Category { Name = "Test Category", Description = "Test Description" };
            var createdCategory = new Category
            {
                Id = 1,
                Name = "Test Category",
                Description = "Test Description",
                CreatedDate = DateTime.UtcNow,
                UpdatedDate = DateTime.UtcNow
            };

            _categoryRepository.AddAsync(Arg.Any<Category>()).Returns(createdCategory);
            _categoryRepository.GetByIdAsync(1).Returns(createdCategory);
            _categoryRepository.UpdateAsync(Arg.Any<Category>()).Returns(createdCategory);
            _categoryRepository.DeleteAsync(1).Returns(true);

            // Act & Assert - Create
            var created = await _categoryService.CreateCategoryAsync(newCategory);
            Assert.NotNull(created);
            Assert.Equal(1, created.Id);

            // Act & Assert - Get
            var retrieved = await _categoryService.GetCategoryByIdAsync(1);
            Assert.NotNull(retrieved);
            Assert.Equal(1, retrieved.Id);

            // Act & Assert - Update
            var updated = await _categoryService.UpdateCategoryAsync(1, new Category
            {
                Name = "Updated Name",
                Description = "Updated Description"
            });
            Assert.NotNull(updated);

            // Act & Assert - Delete
            var deleted = await _categoryService.DeleteCategoryAsync(1);
            Assert.True(deleted);
        }

        #endregion

        #region Additional Edge Case Tests

        [Fact]
        public async Task CreateCategoryAsync_ShouldHandleEmptyName_WhenValidationAllows()
        {
            // Arrange
            var newCategory = new Category
            {
                Name = "",
                Description = "Category with empty name"
            };

            _categoryRepository.AddAsync(Arg.Any<Category>()).Returns(newCategory);

            // Act
            var result = await _categoryService.CreateCategoryAsync(newCategory);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("", result.Name);
            await _categoryRepository.Received(1).AddAsync(Arg.Any<Category>());
        }

        [Fact]
        public async Task CreateCategoryAsync_ShouldHandleNullDescription()
        {
            // Arrange
            var newCategory = new Category
            {
                Name = "Test Category",
                Description = null
            };

            _categoryRepository.AddAsync(Arg.Any<Category>()).Returns(newCategory);

            // Act
            var result = await _categoryService.CreateCategoryAsync(newCategory);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Test Category", result.Name);
            Assert.Null(result.Description);
            await _categoryRepository.Received(1).AddAsync(Arg.Any<Category>());
        }

        [Fact]
        public async Task UpdateCategoryAsync_ShouldHandlePartialUpdate()
        {
            // Arrange
            var categoryId = 1;
            var existingCategory = new Category
            {
                Id = categoryId,
                Name = "Original Name",
                Description = "Original Description",
                CreatedDate = DateTime.UtcNow.AddDays(-1),
                UpdatedDate = DateTime.UtcNow.AddDays(-1)
            };

            var updateCategory = new Category
            {
                Name = "Updated Name",
                Description = null // Testing null description update
            };

            _categoryRepository.GetByIdAsync(categoryId).Returns(existingCategory);
            _categoryRepository.UpdateAsync(existingCategory).Returns(existingCategory);

            // Act
            var result = await _categoryService.UpdateCategoryAsync(categoryId, updateCategory);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Updated Name", result.Name);
            Assert.Null(result.Description);
        }

        #endregion
    }
}