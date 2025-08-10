using InventoryManagement.Models;
using InventoryManagement.Models.DTO;
using InventoryManagement.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace CourseWork.Controllers
{
    [Route("api/[controller]")]
    [Authorize]
    [EnableRateLimiting("CommonPolicy")]
    [ApiController]
    public class InventoryController : ControllerBase
    {
        private readonly IInventoryService _inventoryService;

        public InventoryController(IInventoryService inventoryService)
        {
            _inventoryService = inventoryService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<InventoryItem>>> GetInventory()
        {
            var inventory = await _inventoryService.GetInventoryWithProductsAsync();
            return Ok(inventory);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<InventoryItem>> GetInventoryItem(int id)
        {
            var item = await _inventoryService.GetInventoryByIdAsync(id);
            if (item == null)
                return NotFound();

            return Ok(item);
        }

        [HttpGet("product/{productId}")]
        public async Task<ActionResult<InventoryItem>> GetInventoryByProduct(int productId)
        {
            var item = await _inventoryService.GetInventoryByProductIdAsync(productId);
            if (item == null)
                return NotFound();

            return Ok(item);
        }

        [HttpGet("low-stock")]
        public async Task<ActionResult<IEnumerable<InventoryItem>>> GetLowStockItems()
        {
            var items = await _inventoryService.GetLowStockItemsAsync();
            return Ok(items);
        }

        [HttpPost]
        public async Task<ActionResult<InventoryItem>> CreateInventoryItem(InventoryItemCreateDTO inventoryItem)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var createdItem = await _inventoryService.CreateInventoryItemAsync(inventoryItem);
            return CreatedAtAction(nameof(GetInventoryItem), new { id = createdItem.Id }, createdItem);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<InventoryItem>> UpdateInventoryItem(int id, InventoryItemUpdateDTO inventoryItem)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var updatedItem = await _inventoryService.UpdateInventoryItemAsync(id, inventoryItem);
            if (updatedItem == null)
                return NotFound();

            return Ok(updatedItem);
        }

        [HttpPut("update-stock/{productId}")]
        public async Task<IActionResult> UpdateStock(int productId, [FromBody] int quantity)
        {
            var result = await _inventoryService.UpdateStockAsync(productId, quantity);
            if (!result)
                return NotFound();

            return Ok(new { message = "Stock updated successfully" });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteInventoryItem(int id)
        {
            var result = await _inventoryService.DeleteInventoryItemAsync(id);
            if (!result)
                return NotFound();

            return NoContent();
        }
    }
}
