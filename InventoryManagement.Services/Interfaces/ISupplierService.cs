using InventoryManagement.Models;
using InventoryManagement.Models.DTO;

namespace InventoryManagement.Services.Interfaces
{
    public interface ISupplierService
    {
        Task<IEnumerable<Supplier>> GetAllSuppliersAsync();
        Task<Supplier> GetSupplierByIdAsync(int id);
        Task<Supplier> CreateSupplierAsync(SupplierCreateDTO supplier);
        Task<Supplier> UpdateSupplierAsync(int id, SupplierUpdateDTO supplier);
        Task<bool> DeleteSupplierAsync(int id);
        Task<IEnumerable<Supplier>> GetSuppliersWithProductsAsync();
    }
}
