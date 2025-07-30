using InventoryManagement.Data.Repositories.Interfaces;
using InventoryManagement.Models;
using InventoryManagement.Services.Interfaces;
using InventoryManagement.Services.Utility;

namespace InventoryManagement.Services
{
    /// <summary>
    /// Service class to handle business logic for Supplier-related operations.
    /// </summary>
    public class SupplierService : ISupplierService
    {
        private readonly ISupplierRepository _supplierRepository;
        private readonly ILoggerService<SupplierService> _logger;

        public SupplierService(ISupplierRepository supplierRepository, ILoggerService<SupplierService> logger)
        {
            _supplierRepository = supplierRepository;
            _logger = logger;
        }

        /// <summary>
        /// Retrieves all suppliers from the database.
        /// </summary>
        /// <returns>A list of Supplier objects.</returns>
        public async Task<IEnumerable<Supplier>> GetAllSuppliersAsync()
        {
            try
            {
                return await _supplierRepository.GetAllAsync();
            }
            catch (Exception ex)
            {
                _logger.LogException("Internal server Error", ex);
                throw new Exception("Internal server Error", ex);
            }
        }

        /// <summary>
        /// Retrieves a specific supplier by its ID.
        /// </summary>
        /// <param name="id">The ID of the supplier.</param>
        /// <returns>The Supplier object if found; otherwise, null.</returns>
        public async Task<Supplier> GetSupplierByIdAsync(int id)
        {
            try
            {
                return await _supplierRepository.GetByIdAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogException("Internal server Error", ex);
                throw new Exception("Internal server Error", ex);
            }
        }

        /// <summary>
        /// Creates a new supplier record in the database.
        /// </summary>
        /// <param name="supplier">The supplier object to be created.</param>
        /// <returns>The created Supplier object.</returns>
        public async Task<Supplier> CreateSupplierAsync(Supplier supplier)
        {
            try
            {
                supplier.CreatedDate = DateTime.UtcNow;
                supplier.UpdatedDate = DateTime.UtcNow;
                return await _supplierRepository.AddAsync(supplier);
            }
            catch (Exception ex)
            {
                _logger.LogException("Internal server Error", ex);
                throw new Exception("Internal server Error", ex);
            }
        }

        /// <summary>
        /// Updates an existing supplier record.
        /// </summary>
        /// <param name="id">The ID of the supplier to update.</param>
        /// <param name="supplier">The supplier object containing updated values.</param>
        /// <returns>The updated Supplier object if the update was successful; otherwise, null.</returns>
        public async Task<Supplier> UpdateSupplierAsync(int id, Supplier supplier)
        {
            try
            {
                var existingSupplier = await _supplierRepository.GetByIdAsync(id);
                if (existingSupplier == null) return null;

                existingSupplier.Name = supplier.Name;
                existingSupplier.ContactPerson = supplier.ContactPerson;
                existingSupplier.Phone = supplier.Phone;
                existingSupplier.Email = supplier.Email;
                existingSupplier.Address = supplier.Address;
                existingSupplier.UpdatedDate = DateTime.UtcNow;

                return await _supplierRepository.UpdateAsync(existingSupplier);
            }
            catch (Exception ex)
            {
                _logger.LogException("Internal server Error", ex);
                throw new Exception("Internal server Error", ex);
            }
        }

        /// <summary>
        /// Deletes a supplier from the database based on its ID.
        /// </summary>
        /// <param name="id">The ID of the supplier to delete.</param>
        /// <returns>True if the deletion was successful; otherwise, false.</returns>
        public async Task<bool> DeleteSupplierAsync(int id)
        {
            try
            {
                return await _supplierRepository.DeleteAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogException("Internal server Error", ex);
                throw new Exception("Internal server Error", ex);
            }
        }

        /// <summary>
        /// Retrieves all suppliers along with their associated products.
        /// </summary>
        /// <returns>A list of Supplier objects with included product data.</returns>
        public async Task<IEnumerable<Supplier>> GetSuppliersWithProductsAsync()
        {
            try
            {
                return await _supplierRepository.GetSuppliersWithProductsAsync();
            }
            catch (Exception ex)
            {
                _logger.LogException("Internal server Error", ex);
                throw new Exception("Internal server Error", ex);
            }
        }
    }
}
