using InventoryManagement.Data.Repositories.Interfaces;
using InventoryManagement.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventoryManagement.Data.Repositories
{
    public class SupplierRepository : Repository<Supplier>, ISupplierRepository
    {
        public SupplierRepository(AppDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Supplier>> GetSuppliersWithProductsAsync()
        {
            return await _context.Suppliers
                .Include(s => s.Products)
                .ToListAsync();
        }
    }
}
