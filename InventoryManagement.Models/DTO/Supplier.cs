namespace InventoryManagement.Models.DTO
{
    public record SupplierCreateDTO(string Name, string ContactPerson, string Phone, string Email, string Address);

    public record SupplierUpdateDTO(int Id, string Name, string ContactPerson, string Phone, string Email, string Address);
}
