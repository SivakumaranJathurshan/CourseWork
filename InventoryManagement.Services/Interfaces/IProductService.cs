using InventoryManagement.Models;
using InventoryManagement.Models.DTO;

namespace InventoryManagement.Services.Interfaces
{
    public interface IProductService
    {
        Task<IEnumerable<Product>> GetAllProductsAsync();
        Task<Product> GetProductByIdAsync(int id);
        Task<Product> CreateProductAsync(ProductCreateDTO product);
        Task<Product> UpdateProductAsync(int id, ProductUpdateDTO product);
        Task<bool> DeleteProductAsync(int id);
        Task<IEnumerable<Product>> GetProductsWithDetailsAsync();
        Task<IEnumerable<Product>> GetProductsByCategoryAsync(int categoryId);
        Task<Product> GetProductBySkuAsync(string sku);
    }
}
