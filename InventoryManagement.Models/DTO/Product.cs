namespace InventoryManagement.Models.DTO
{
    public record ProductCreateDTO(string Name, string Description, string SKU, decimal Price, int CategoryId, int SupplierId);

    public record ProductUpdateDTO(int Id, string Name, string Description, string SKU, decimal Price, int CategoryId, int SupplierId);
}
