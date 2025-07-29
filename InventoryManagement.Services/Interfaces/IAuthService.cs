using InventoryManagement.Models;
using InventoryManagement.Models.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventoryManagement.Services.Interfaces
{
    public interface IAuthService
    {
        (bool Success, string? Error) Register(RegisterDTO request);

        (bool Success, string? Token, string? Error) Signin(LoginDTO request);
    }
}
