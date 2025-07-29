using InventoryManagement.Models;
using InventoryManagement.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CourseWork.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SuppliersController : ControllerBase
    {
        private readonly ISupplierService _supplierService;

        public SuppliersController(ISupplierService supplierService)
        {
            _supplierService = supplierService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Supplier>>> GetSuppliers()
        {
            var suppliers = await _supplierService.GetAllSuppliersAsync();
            return Ok(suppliers);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Supplier>> GetSupplier(int id)
        {
            var supplier = await _supplierService.GetSupplierByIdAsync(id);
            if (supplier == null)
                return NotFound();

            return Ok(supplier);
        }

        [HttpGet("with-products")]
        public async Task<ActionResult<IEnumerable<Supplier>>> GetSuppliersWithProducts()
        {
            var suppliers = await _supplierService.GetSuppliersWithProductsAsync();
            return Ok(suppliers);
        }

        [HttpPost]
        public async Task<ActionResult<Supplier>> CreateSupplier(Supplier supplier)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var createdSupplier = await _supplierService.CreateSupplierAsync(supplier);
            return CreatedAtAction(nameof(GetSupplier), new { id = createdSupplier.Id }, createdSupplier);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<Supplier>> UpdateSupplier(int id, Supplier supplier)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var updatedSupplier = await _supplierService.UpdateSupplierAsync(id, supplier);
            if (updatedSupplier == null)
                return NotFound();

            return Ok(updatedSupplier);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSupplier(int id)
        {
            var result = await _supplierService.DeleteSupplierAsync(id);
            if (!result)
                return NotFound();

            return NoContent();
        }
    }
}
