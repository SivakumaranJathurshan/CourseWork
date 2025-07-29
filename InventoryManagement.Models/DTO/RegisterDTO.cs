using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventoryManagement.Models.DTO
{
    public record RegisterDTO(string FirstName, string LastName, string Email, string Password, string ConfirmPassword);
}
