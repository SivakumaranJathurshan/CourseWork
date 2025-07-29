using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventoryManagement.Models
{
    public class InventoryItem
    {
        [Key]
        public int Id { get; set; }

        public int ProductId { get; set; }

        [Required]
        public int Quantity { get; set; }

        public int MinimumStock { get; set; }
        public int MaximumStock { get; set; }

        public DateTime LastRestocked { get; set; } = DateTime.UtcNow;
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedDate { get; set; } = DateTime.UtcNow;

        // Navigation property
        public virtual Product Product { get; set; }
    }
}
