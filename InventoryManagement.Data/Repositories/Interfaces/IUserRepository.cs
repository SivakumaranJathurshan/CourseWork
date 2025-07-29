using InventoryManagement.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventoryManagement.Data.Repositories.Interfaces
{
    public interface IUserRepository
    {
        User? GetByEmail(string Email);
        void Add(User user);
    }
}
