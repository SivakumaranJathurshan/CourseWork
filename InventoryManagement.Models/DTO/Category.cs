namespace InventoryManagement.Models.DTO
{
    public record CategoryCreateDTO(string Name, string Description);

    public record CategoryUpdateDTO(int Id, string Name, string Description);
}
