using InventoryManagement.Data.Repositories.Interfaces;
using InventoryManagement.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventoryManagement.Data.Repositories
{
    public class UserRepository : Repository<Category>, IUserRepository
    {
        public UserRepository(AppDbContext context) : base(context) { }

        public void Add(User user)
        {
            _context.Add(user);
            _context.SaveChanges();
        }

        public User? GetByEmail(string Email) =>
            _context.Users.FirstOrDefault(u => u.Email == Email);
    }
}
