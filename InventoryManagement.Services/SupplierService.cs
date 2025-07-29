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
    public class SupplierService : ISupplierService
    {
        private readonly ISupplierRepository _supplierRepository;
        private readonly ILoggerService<SupplierService> _logger;

        public SupplierService(ISupplierRepository supplierRepository, ILoggerService<SupplierService> logger)
        {
            _supplierRepository = supplierRepository;
            _logger = logger;
        }

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

        public async Task<Supplier> GetSupplierByIdAsync(int id)
        {
            try { 
                return await _supplierRepository.GetByIdAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogException("Internal server Error", ex);
                throw new Exception("Internal server Error", ex);
            }
        }

        public async Task<Supplier> CreateSupplierAsync(Supplier supplier)
        {
            try { 
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

        public async Task<IEnumerable<Supplier>> GetSuppliersWithProductsAsync()
        {
            try { 
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
