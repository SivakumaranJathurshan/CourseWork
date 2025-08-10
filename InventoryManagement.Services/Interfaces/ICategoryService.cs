using InventoryManagement.Models;
using InventoryManagement.Models.DTO;

namespace InventoryManagement.Services.Interfaces
{
    public interface ICategoryService
    {
        Task<IEnumerable<Category>> GetAllCategoriesAsync();
        Task<Category> GetCategoryByIdAsync(int id);
        Task<Category> CreateCategoryAsync(CategoryCreateDTO category);
        Task<Category> UpdateCategoryAsync(int id, CategoryUpdateDTO category);
        Task<bool> DeleteCategoryAsync(int id);
        Task<IEnumerable<Category>> GetCategoriesWithProductsAsync();
    }
}
