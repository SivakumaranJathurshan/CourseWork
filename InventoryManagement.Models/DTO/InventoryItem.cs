namespace InventoryManagement.Models.DTO
{
    public record InventoryItemCreateDTO(int ProductId, int Quantity, int MinimumStock, int MaximumStock);

    public record InventoryItemUpdateDTO(int Id, int ProductId, int Quantity, int MinimumStock, int MaximumStock);
}
