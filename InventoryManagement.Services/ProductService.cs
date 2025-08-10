using InventoryManagement.Data.Repositories.Interfaces;
using InventoryManagement.Models;
using InventoryManagement.Models.DTO;
using InventoryManagement.Services.Interfaces;
using InventoryManagement.Services.Utility;

namespace InventoryManagement.Services
{
    /// <summary>
    /// Handles business logic related to product operations.
    /// </summary>
    public class ProductService : IProductService
    {
        private readonly IProductRepository _productRepository;
        private readonly ILoggerService<ProductService> _logger;

        public ProductService(IProductRepository productRepository, ILoggerService<ProductService> logger)
        {
            _productRepository = productRepository;
            _logger = logger;
        }

        /// <summary>
        /// Retrieves all products from the database.
        /// </summary>
        /// <returns>A list of all products.</returns>
        public async Task<IEnumerable<Product>> GetAllProductsAsync()
        {
            try
            {
                return await _productRepository.GetAllAsync();
            }
            catch (Exception ex)
            {
                _logger.LogException("Internal server Error", ex);
                throw new Exception("Internal server Error", ex);
            }
        }

        /// <summary>
        /// Retrieves a product by its ID including its related details (e.g., category, supplier).
        /// </summary>
        /// <param name="id">The ID of the product.</param>
        /// <returns>The product with detailed information.</returns>
        public async Task<Product> GetProductByIdAsync(int id)
        {
            try
            {
                return await _productRepository.GetProductWithDetailsAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogException("Internal server Error", ex);
                throw new Exception("Internal server Error", ex);
            }
        }

        /// <summary>
        /// Creates a new product in the database.
        /// </summary>
        /// <param name="product">The product entity to create.</param>
        /// <returns>The created product entity.</returns>
        public async Task<Product> CreateProductAsync(ProductCreateDTO product)
        {
            try
            {
                Product newProduct = new Product();
                newProduct.Name = product.Name;
                newProduct.Description = product.Description;
                newProduct.SKU = product.SKU;
                newProduct.Price = product.Price;
                newProduct.CategoryId = product.CategoryId;
                newProduct.SupplierId = product.SupplierId;
                newProduct.CreatedDate = DateTime.UtcNow;
                newProduct.UpdatedDate = DateTime.UtcNow;
                return await _productRepository.AddAsync(newProduct);
            }
            catch (Exception ex)
            {
                _logger.LogException("Internal server Error", ex);
                throw new Exception("Internal server Error", ex);
            }
        }

        /// <summary>
        /// Updates an existing product based on its ID.
        /// </summary>
        /// <param name="id">The ID of the product to update.</param>
        /// <param name="product">The updated product data.</param>
        /// <returns>The updated product entity, or null if not found.</returns>
        public async Task<Product> UpdateProductAsync(int id, ProductUpdateDTO product)
        {
            try
            {
                var existingProduct = await _productRepository.GetByIdAsync(id);
                if (existingProduct == null) return null;

                existingProduct.Name = product.Name;
                existingProduct.Description = product.Description;
                existingProduct.SKU = product.SKU;
                existingProduct.Price = product.Price;
                existingProduct.CategoryId = product.CategoryId;
                existingProduct.SupplierId = product.SupplierId;
                existingProduct.UpdatedDate = DateTime.UtcNow;

                return await _productRepository.UpdateAsync(existingProduct);
            }
            catch (Exception ex)
            {
                _logger.LogException("Internal server Error", ex);
                throw new Exception("Internal server Error", ex);
            }
        }

        /// <summary>
        /// Deletes a product by its ID.
        /// </summary>
        /// <param name="id">The ID of the product to delete.</param>
        /// <returns>True if deleted successfully, false otherwise.</returns>
        public async Task<bool> DeleteProductAsync(int id)
        {
            try
            {
                return await _productRepository.DeleteAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogException("Internal server Error", ex);
                throw new Exception("Internal server Error", ex);
            }
        }

        /// <summary>
        /// Retrieves all products along with their category and supplier details.
        /// </summary>
        /// <returns>A list of products with full details.</returns>
        public async Task<IEnumerable<Product>> GetProductsWithDetailsAsync()
        {
            try
            {
                return await _productRepository.GetProductsWithDetailsAsync();
            }
            catch (Exception ex)
            {
                _logger.LogException("Internal server Error", ex);
                throw new Exception("Internal server Error", ex);
            }
        }

        /// <summary>
        /// Retrieves products belonging to a specific category.
        /// </summary>
        /// <param name="categoryId">The ID of the category.</param>
        /// <returns>A list of products in the given category.</returns>
        public async Task<IEnumerable<Product>> GetProductsByCategoryAsync(int categoryId)
        {
            try
            {
                return await _productRepository.GetProductsByCategoryAsync(categoryId);
            }
            catch (Exception ex)
            {
                _logger.LogException("Internal server Error", ex);
                throw new Exception("Internal server Error", ex);
            }
        }

        /// <summary>
        /// Retrieves a product using its SKU (Stock Keeping Unit).
        /// </summary>
        /// <param name="sku">The SKU of the product.</param>
        /// <returns>The product with the matching SKU.</returns>
        public async Task<Product> GetProductBySkuAsync(string sku)
        {
            try
            {
                return await _productRepository.GetProductBySkuAsync(sku);
            }
            catch (Exception ex)
            {
                _logger.LogException("Internal server Error", ex);
                throw new Exception("Internal server Error", ex);
            }
        }
    }
}
