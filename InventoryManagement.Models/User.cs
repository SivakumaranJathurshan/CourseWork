using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace InventoryManagement.Models
{
    public class User
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string FirstName { get; set; }
        [Required]
        public string LastName { get; set; }
        [Required]
        public string Email { get; set; }
        public string PasswordHash { get; set; }
        
        public DateTime RegisteredOn { get; set; }
        public DateTime LastLoginOn { get; set ; }
    }
}