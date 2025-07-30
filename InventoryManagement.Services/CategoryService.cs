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
    /// <summary>
    /// Service class responsible for handling business logic related to product categories.
    /// </summary>
    public class CategoryService : ICategoryService
    {
        private readonly ICategoryRepository _categoryRepository;
        private readonly ILoggerService<CategoryService> _logger;

        /// <summary>
        /// Constructor for CategoryService.
        /// </summary>
        /// <param name="categoryRepository">The category repository for data access.</param>
        /// <param name="logger">The logger service for error logging.</param>
        public CategoryService(ICategoryRepository categoryRepository, ILoggerService<CategoryService> logger)
        {
            _categoryRepository = categoryRepository;
            _logger = logger;
        }

        /// <summary>
        /// Retrieves all categories asynchronously.
        /// </summary>
        /// <returns>An enumerable collection of all categories.</returns>
        public async Task<IEnumerable<Category>> GetAllCategoriesAsync()
        {
            try
            {
                return await _categoryRepository.GetAllAsync();
            }
            catch (Exception ex)
            {
                _logger.LogException("Internal server Error", ex);
                throw new Exception("Internal server Error", ex);
            }
        }

        /// <summary>
        /// Retrieves a single category by its unique ID.
        /// </summary>
        /// <param name="id">The ID of the category.</param>
        /// <returns>The category with the given ID, or null if not found.</returns>
        public async Task<Category> GetCategoryByIdAsync(int id)
        {
            try
            {
                return await _categoryRepository.GetByIdAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogException("Internal server Error", ex);
                throw new Exception("Internal server Error", ex);
            }
        }

        /// <summary>
        /// Creates a new category with the provided details.
        /// </summary>
        /// <param name="category">The category object to create.</param>
        /// <returns>The created category with updated information.</returns>
        public async Task<Category> CreateCategoryAsync(Category category)
        {
            try
            {
                category.CreatedDate = DateTime.UtcNow;
                category.UpdatedDate = DateTime.UtcNow;
                return await _categoryRepository.AddAsync(category);
            }
            catch (Exception ex)
            {
                _logger.LogException("Internal server Error", ex);
                throw new Exception("Internal server Error", ex);
            }
        }

        /// <summary>
        /// Updates an existing category based on the provided ID and new details.
        /// </summary>
        /// <param name="id">The ID of the category to update.</param>
        /// <param name="category">The updated category data.</param>
        /// <returns>The updated category, or null if not found.</returns>
        public async Task<Category> UpdateCategoryAsync(int id, Category category)
        {
            try { 
                var existingCategory = await _categoryRepository.GetByIdAsync(id);
                if (existingCategory == null) return null;

                existingCategory.Name = category.Name;
                existingCategory.Description = category.Description;
                existingCategory.UpdatedDate = DateTime.UtcNow;

                return await _categoryRepository.UpdateAsync(existingCategory);
            }
            catch (Exception ex)
            {
                _logger.LogException("Internal server Error", ex);
                throw new Exception("Internal server Error", ex);
            }
        }

        /// <summary>
        /// Deletes a category by its ID.
        /// </summary>
        /// <param name="id">The ID of the category to delete.</param>
        /// <returns>True if the category was deleted successfully, otherwise false.</returns>
        public async Task<bool> DeleteCategoryAsync(int id)
        {
            try { 
                return await _categoryRepository.DeleteAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogException("Internal server Error", ex);
                throw new Exception("Internal server Error", ex);
            }
        }

        /// <summary>
        /// Retrieves all categories along with their associated products.
        /// </summary>
        /// <returns>A collection of categories, each including a list of its products.</returns>
        public async Task<IEnumerable<Category>> GetCategoriesWithProductsAsync()
        {
            try { 
                return await _categoryRepository.GetCategoriesWithProductsAsync();
            }
            catch (Exception ex)
            {
                _logger.LogException("Internal server Error", ex);
                throw new Exception("Internal server Error", ex);
            }
        }
    }
}
