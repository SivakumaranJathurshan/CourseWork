namespace InventoryManagement.Models.DTO
{
    public record OrderCreateDTO(string OrderNumber, string CustomerName, string CustomerAddress, string CustomerPhone, OrderStatus Status, decimal TotalAmount);

    public record OrderUpdateDTO(int Id, string OrderNumber, string CustomerName, string CustomerAddress, string CustomerPhone, OrderStatus Status, decimal TotalAmount);
}
